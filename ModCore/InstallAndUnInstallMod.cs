using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ModInstaller.Properties;
using RDR2_RPF_Tool.Core;

namespace ModInstaller.ModCore
{
	// Token: 0x02000011 RID: 17
	public class InstallAndUnInstallMod
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00003A74 File Offset: 0x00001C74
		public void Installmod()
		{
			byte[] FileModByte = Resources.ModFile;
			Stream FileModStream = new MemoryStream(FileModByte);
			BinaryReader FileMod = new BinaryReader(FileModStream);
			InstallAndUnInstallMod.MaxBarValue = 33;
			List<FileInfo> FilesInModFile = new FileModAnalyzer().GetFilesInModFile(FileMod);
			new MakeBackup().GetFilesInModFile(FilesInModFile);
			for (int i = 0; i < FilesInModFile.Count; i++)
			{
				bool flag = FilesInModFile[i].FileCommand == FileEnum.copy;
				if (flag)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(FilesInModFile[i].FilePath));
					bool flag2 = File.Exists(FilesInModFile[i].FilePath);
					if (flag2)
					{
						try
						{
							File.Delete(FilesInModFile[i].FilePath);
						}
						catch
						{
							throw new Exception("حدث خطاء غير معروف عند محاولة تعديل الملفات.");
						}
					}
					InstallAndUnInstallMod.WriteFile(FileMod, FilesInModFile, i);
					bool flag3 = FilesInModFile[i].FileName == "Hide";
					if (flag3)
					{
						this.FileEdit(FilesInModFile[i].FilePath);
					}
				}
				bool flag4 = FilesInModFile[i].FileCommand == FileEnum.delete;
				if (flag4)
				{
					bool flag5 = File.Exists(FilesInModFile[i].FilePath);
					if (flag5)
					{
						File.Delete(FilesInModFile[i].FilePath);
					}
				}
				bool flag6 = FilesInModFile[i].FileCommand == FileEnum.EditTages;
				if (flag6)
				{
					bool flag7 = File.Exists(FilesInModFile[i].FilePath);
					if (flag7)
					{
						FileStream fileStream = new FileStream(FilesInModFile[i].FilePath, FileMode.Open, FileAccess.ReadWrite);
						BinaryReader binary = new BinaryReader(fileStream);
						BinaryWriter binaryWriter = new BinaryWriter(fileStream);
						List<ulong> Ids = new List<ulong>();
						FileMod.BaseStream.Position = FilesInModFile[i - 1].FileOffet;
						BinaryReader FileTage = new BinaryReader(new MemoryStream(FileMod.ReadBytes((int)FilesInModFile[i - 1].FileSize)));
						FileTage.BaseStream.Position = 20L;
						int count = FileTage.ReadInt32();
						for (int j = 0; j < count; j++)
						{
							Ids.Add(FileTage.ReadUInt64());
							FileTage.BaseStream.Position += 12L;
						}
						binary.BaseStream.Position = 20L;
						count = binary.ReadInt32();
						for (int k = 0; k < count; k++)
						{
							long ThisPos = binary.BaseStream.Position;
							ulong Tag = binary.ReadUInt64();
							binary.BaseStream.Position += 12L;
							long LastPos = binary.BaseStream.Position;
							bool flag8 = Ids.IndexOf(Tag) != -1;
							if (flag8)
							{
								binaryWriter.BaseStream.Position = ThisPos;
								binaryWriter.Write(0L);
							}
							binary.BaseStream.Position = LastPos;
						}
					}
				}
				bool flag9 = FilesInModFile[i].FileCommand == FileEnum.unrealengine;
				if (flag9)
				{
					string[] FilesPak = Directory.GetFiles(FilesInModFile[i].FilePath, "*.pak", SearchOption.AllDirectories);
					string FilePakPath = Path.ChangeExtension(FilesPak[0], null);
					FileMod.BaseStream.Position = FilesInModFile[i].FileOffet;
					File.WriteAllBytes(FilePakPath + "_P.Pak", FileMod.ReadBytes((int)FilesInModFile[i].FileSize));
					bool flag10 = File.Exists(FilePakPath + ".sig");
					if (flag10)
					{
						File.WriteAllBytes(FilePakPath + "_P.sig", File.ReadAllBytes(FilePakPath + ".sig"));
					}
				}
				bool flag11 = FilesInModFile[i].FileCommand == FileEnum.UpdateRPF;
				if (flag11)
				{
					FileMod.BaseStream.Seek(FilesInModFile[i].FileOffet, SeekOrigin.Begin);
					RPF8 rpf = RPF8.Load(FilesInModFile[i].RpfPath);
					bool makeBackup = FilesInModFile[i].MakeBackup;
					if (makeBackup)
					{
						rpf.RPFCFile = RPFC.Load(RPF8.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appdata0_update.rpf")));
					}
					else
					{
						rpf.RPFCFile = RPFC.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pfm.dat"));
					}
					RPF8 rpf2 = RPF8.Load(FilesInModFile[i].FileName, rpf.GetFile(FilesInModFile[i].FileName, true));
					rpf2.ImportFile(FilesInModFile[i].FilePath, FileMod.ReadBytes((int)FilesInModFile[i].FileSize));
					rpf2.ReBuild();
					rpf.ImportFile(rpf2.FilePath, rpf2.Rpf8StreamFile.ToArray());
					bool flag12 = rpf.RPFCFile.rpf8 != null;
					if (flag12)
					{
						rpf.RPFCFile.rpf8.Destroy();
					}
					rpf.Destroy();
					rpf2.Destroy();
				}
				bool flag13 = FilesInModFile[i].FileCommand == FileEnum.ReplaceStrings;
				if (flag13)
				{
					FileMod.BaseStream.Seek(FilesInModFile[i].FileOffet, SeekOrigin.Begin);
					RPF8 rpf3 = RPF8.Load(FilesInModFile[i].RpfPath);
					bool makeBackup2 = FilesInModFile[i].MakeBackup;
					if (makeBackup2)
					{
						rpf3.RPFCFile = RPFC.Load(RPF8.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appdata0_update.rpf")));
					}
					else
					{
						rpf3.RPFCFile = RPFC.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pfm.dat"));
					}
					RPF8 rpf4 = RPF8.Load(FilesInModFile[i].FileName, rpf3.GetFile(FilesInModFile[i].FileName, true));
					string[] textfile = Encoding.UTF8.GetString(MakeBackup._Bytes(Compression.DeflateDecompress(FileMod.ReadBytes((int)FilesInModFile[i].FileSize)))).Split(new char[]
					{
						'\r',
						'\n'
					}, StringSplitOptions.RemoveEmptyEntries);
					Dictionary<uint, string> keyValuePairs = new Dictionary<uint, string>();
					foreach (string str in (from x in textfile
					where x.Contains("=")
					select x).ToArray<string>())
					{
						string[] Line = str.Split(new char[]
						{
							'='
						}, 2);
						Line[0] = Line[0].Trim();
						bool flag14 = Line[0].StartsWith("0x");
						uint hash;
						if (flag14)
						{
							hash = Convert.ToUInt32(Line[0], 16);
						}
						else
						{
							hash = JOAATHash.Calc(Line[0].ToLower());
						}
						string text = Line[1];
						bool flag15 = !keyValuePairs.ContainsKey(hash);
						if (flag15)
						{
							keyValuePairs.Add(hash, text);
						}
					}
					for (int index = 0; index < rpf4.Entries.Count; index++)
					{
						KeyValuePair<string, RPF8.Entry> entry = rpf4.Entries.ElementAt(index);
						rpf4.ImportFile(entry.Key, DataBaseFile.ImportTexts(rpf4.GetFile(entry.Key, true), keyValuePairs));
						//ProcessPage.progressDes.Report("جارٍ إدخال نصوص اللعبة...");
						//ProcessPage.progress.Report((index + 1) * 100 / rpf4.Entries.Count);
					}
					rpf4.ReBuild();
					rpf3.ImportFile(rpf4.FilePath, rpf4.Rpf8StreamFile.ToArray());
					bool flag16 = rpf3.RPFCFile.rpf8 != null;
					if (flag16)
					{
						rpf3.RPFCFile.rpf8.Destroy();
					}
					rpf3.Destroy();
					rpf4.Destroy();
				}
				//ProcessPage.progressDes.Report("جارٍ تثبيت التعريب...");
				//ProcessPage.progress.Report((i + 1) * 100 / FilesInModFile.Count);
				Thread.Sleep(100);
			}
			FileMod.Close();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000042A4 File Offset: 0x000024A4
		private static void WriteFile(BinaryReader FileMod, List<FileInfo> FilesInModFile, int n)
		{
			FileMod.BaseStream.Seek(FilesInModFile[n].FileOffet, SeekOrigin.Begin);
			using (BinaryWriter destFile = new BinaryWriter(File.Open(FilesInModFile[n].FilePath, FileMode.Create, FileAccess.Write)))
			{
				int chunkSize = 104857600;
				long length = FilesInModFile[n].FileSize;
				byte[] buffer = new byte[chunkSize];
				while (length > 0L)
				{
					int bytesRead = FileMod.BaseStream.Read(buffer, 0, (int)Math.Min(length, (long)chunkSize));
					destFile.Write(buffer, 0, bytesRead);
					length -= (long)bytesRead;
				}
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004358 File Offset: 0x00002558
		public void UnInstallmod()
		{
			BinaryReader FileMod = new BinaryReader(new FileStream("Mod Backup/ModBackup.as", FileMode.Open, FileAccess.ReadWrite));
			InstallAndUnInstallMod.BarValue = 0;
			InstallAndUnInstallMod.MaxBarValue = 50;
			List<FileInfo> FilesInModFile = new FileModAnalyzer().GetFilesInModFile(FileMod);
			for (int i = 0; i < FilesInModFile.Count; i++)
			{
				bool flag = FilesInModFile[i].FileCommand == FileEnum.copy;
				if (flag)
				{
					bool flag2 = File.Exists(FilesInModFile[i].FilePath);
					if (flag2)
					{
						try
						{
							File.Delete(FilesInModFile[i].FilePath);
						}
						catch
						{
							throw new Exception("حدث خطأ غير معروف عند محاولة تعديل الملفات.");
						}
					}
					FileMod.BaseStream.Position = FilesInModFile[i].FileOffet;
					InstallAndUnInstallMod.WriteFile(FileMod, FilesInModFile, i);
				}
				bool flag3 = FilesInModFile[i].FileCommand == FileEnum.delete;
				if (flag3)
				{
					bool flag4 = File.Exists(FilesInModFile[i].FilePath);
					if (flag4)
					{
						File.Delete(FilesInModFile[i].FilePath);
					}
				}
				//ProcessPage.progressDes.Report("جارٍ إلغاء تثبيت التعريب...");
				//ProcessPage.progress.Report((i + 1) * 100 / FilesInModFile.Count);
				Thread.Sleep(100);
			}
			FileMod.Close();
			Directory.Delete("Mod Backup", true);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000044B8 File Offset: 0x000026B8
		protected void FileEdit(string path)
		{
            new System.IO.FileInfo(path).Attributes = (FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive);
        }

		
		public static int MaxBarValue;

		
		public static int BarValue;
	}
}
