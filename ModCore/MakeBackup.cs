using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using RDR2_RPF_Tool.Core;

namespace ModInstaller.ModCore
{
	// Token: 0x02000010 RID: 16
	public class MakeBackup
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00003178 File Offset: 0x00001378
		public void GetFilesInModFile(List<FileInfo> FileInModFile)
		{
			Directory.CreateDirectory("Mod Backup");
			BinaryWriter FileBackUp = new BinaryWriter(new FileStream("Mod Backup/ModBackup.as", FileMode.Create, FileAccess.ReadWrite));
			int FileCount = 0;
			try
			{
				FileBackUp.Write(FileCount);
				List<string> Rpf8List = new List<string>();
				for (int i = 0; i < FileInModFile.Count; i++)
				{
					switch (FileInModFile[i].FileCommand)
					{
					case FileEnum.copy:
					{
						bool flag = FileInModFile[i].MakeBackup && File.Exists(FileInModFile[i].FilePath);
						if (flag)
						{
							FileBackUp.Write((int)FileInModFile[i].FileCommand);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FileName));
							FileBackUp.Write(false);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FilePath));
							MakeBackup.WriteFile(FileInModFile, FileBackUp, i, false);
							FileCount++;
						}
						else
						{
							FileBackUp.Write(2);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FileName));
							FileBackUp.Write(false);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FilePath));
							MakeBackup.WriteFile(FileInModFile, FileBackUp, i, true);
							FileCount++;
						}
						break;
					}
					case FileEnum.delete:
					{
						bool flag2 = File.Exists(FileInModFile[i].FilePath);
						if (flag2)
						{
							FileBackUp.Write(1);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FileName));
							FileBackUp.Write(false);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FilePath));
							MakeBackup.WriteFile(FileInModFile, FileBackUp, i, false);
							FileCount++;
						}
						break;
					}
					case FileEnum.unrealengine:
					{
						string[] FilesPak = Directory.GetFiles(FileInModFile[i].FilePath, "*.pak", SearchOption.AllDirectories);
						string FilePakPath = Path.ChangeExtension(FilesPak[0], null);
						bool BackupPak = false;
						bool BackupSig = false;
						bool flag3 = File.Exists(FilePakPath + ".Pak");
						if (flag3)
						{
							bool flag4 = File.Exists(FilePakPath + "_P.Pak");
							if (flag4)
							{
								BackupPak = true;
							}
						}
						bool flag5 = File.Exists(FilePakPath + ".sig");
						if (flag5)
						{
							bool flag6 = File.Exists(FilePakPath + "_P.sig");
							if (flag6)
							{
								BackupSig = true;
							}
						}
						bool flag7 = BackupSig;
						if (flag7)
						{
							byte[] FileBackupGet = File.ReadAllBytes(FilePakPath + "_P.sig");
							FileBackUp.Write(1);
							FileBackUp.Write(MakeBackup._String("FileSig"));
							FileBackUp.Write(false);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FilePath + "\\" + Path.GetFileNameWithoutExtension(FilePakPath) + "_P.sig"));
							FileBackUp.Write(FileBackupGet.Length);
							FileBackUp.Write(FileBackupGet);
							FileCount++;
						}
						else
						{
							FileBackUp.Write(2);
							FileBackUp.Write(MakeBackup._String("FileSig"));
							FileBackUp.Write(false);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FilePath + "\\" + Path.GetFileNameWithoutExtension(FilePakPath) + "_P.sig"));
							FileBackUp.Write(0);
							FileCount++;
						}
						bool flag8 = BackupPak;
						if (flag8)
						{
							byte[] FileBackupGet = File.ReadAllBytes(FilePakPath + "_P.Pak");
							FileBackUp.Write(1);
							FileBackUp.Write(MakeBackup._String("FilePak"));
							FileBackUp.Write(false);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FilePath + "\\" + Path.GetFileNameWithoutExtension(FilePakPath) + "_P.Pak"));
							FileBackUp.Write(FileBackupGet.Length);
							FileBackUp.Write(FileBackupGet);
							FileCount++;
						}
						else
						{
							FileBackUp.Write(2);
							FileBackUp.Write(MakeBackup._String("FilePak"));
							FileBackUp.Write(false);
							FileBackUp.Write(MakeBackup._String(FileInModFile[i].FilePath + "\\" + Path.GetFileNameWithoutExtension(FilePakPath) + "_P.Pak"));
							FileBackUp.Write(0);
							FileCount++;
						}
						break;
					}
					case FileEnum.MakeSureExists:
					{
						bool flag9 = !File.Exists(FileInModFile[i].FilePath);
						if (flag9)
						{
							throw new Exception(" Yükleyici bu dosyayı bulamadı: \n" + Path.GetFullPath(FileInModFile[i].FilePath));
						}
						FileBackUp.Write(6);
						FileBackUp.Write(MakeBackup._String(FileInModFile[i].FileName));
						FileBackUp.Write(false);
						FileBackUp.Write(MakeBackup._String(FileInModFile[i].FilePath));
						MakeBackup.WriteFile(FileInModFile, FileBackUp, i, true);
						FileCount++;
						break;
					}
					case FileEnum.MakeBackUp:
					{
						bool flag10 = File.Exists(FileInModFile[i].FilePath);
						if (flag10)
						{
							FileCount = MakeBackup.MakeBackUp(FileInModFile, FileBackUp, FileCount, i);
						}
						break;
					}
					case FileEnum.UpdateRPF:
					case FileEnum.ReplaceStrings:
					{
						FileInModFile[i].FileName = RPFHelper.FileHash(FileInModFile[i].FileName, FileInModFile[i].MakeBackup ? Platform.Pc : Platform.Ps4);
						FileInModFile[i].FilePath = RPFHelper.FileHash(FileInModFile[i].FilePath, FileInModFile[i].MakeBackup ? Platform.Pc : Platform.Ps4);
						IEnumerable<string> Files = from x in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "update*.rpf")
						where Path.GetFileNameWithoutExtension(x).Length <= 10
						select x;
						bool flag11 = Files.Count<string>() == 0;
						if (flag11)
						{
							throw new Exception("Yükleyici oyunun güncelleme dosyasını bulamadı");
						}
						foreach (string path in Files)
						{
							RPF8 rpf = RPF8.Load(path);
							bool flag12 = rpf.Entries.ContainsKey(FileInModFile[i].FileName);
							if (flag12)
							{
								FileInModFile[i].RpfPath = path;
								rpf.Destroy();
								bool flag13 = !Rpf8List.Contains(path);
								if (flag13)
								{
									Rpf8List.Add(path);
									FileInfo fileInfo = new FileInfo();
									fileInfo.FilePath = Path.GetFileName(path);
									FileCount = MakeBackup.MakeBackUp(new List<FileInfo>
									{
										fileInfo
									}, FileBackUp, FileCount, 0);
								}
							}
							rpf.Destroy();
						}
						bool flag14 = Rpf8List.Count<string>() == 0;
						if (flag14)
						{
							throw new Exception("Yükleyici, oyunun güncelleme dosyası içinde hedef dosyaları bulamadı");

                        }
						break;
					}
					}
					//ProcessPage.progressDes.Report("جارٍ عمل نسخه احتياطية من الملفات التي سيتم التعديل عليها...");
					//ProcessPage.progress.Report((i + 1) * 100 / FileInModFile.Count);
					Thread.Sleep(100);
				}
			}
			catch (Exception ex)
			{
				FileBackUp.Close();
				Directory.Delete("Mod Backup", true);
				throw new Exception(ex.Message);
			}
			FileBackUp.BaseStream.Position = 0L;
			FileBackUp.Write(FileCount);
			FileBackUp.Close();
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000038E4 File Offset: 0x00001AE4
		private static int MakeBackUp(List<FileInfo> FileInModFile, BinaryWriter FileBackUp, int FileCount, int n)
		{
			FileBackUp.Write(1);
			FileBackUp.Write(MakeBackup._String(FileInModFile[n].FileName));
			FileBackUp.Write(false);
			FileBackUp.Write(MakeBackup._String(FileInModFile[n].FilePath));
			MakeBackup.WriteFile(FileInModFile, FileBackUp, n, false);
			FileCount++;
			return FileCount;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003948 File Offset: 0x00001B48
		private static void WriteFile(List<FileInfo> FileInModFile, BinaryWriter FileBackUp, int n, bool Temp = false)
		{
			if (Temp)
			{
				FileBackUp.Write(0L);
			}
			else
			{
				using (FileStream FileStream = File.OpenRead(FileInModFile[n].FilePath))
				{
					FileBackUp.Write(FileStream.Length);
					byte[] array = new byte[104857600];
					int count;
					while ((count = FileStream.Read(array, 0, array.Length)) != 0)
					{
						FileBackUp.Write(array, 0, count);
					}
				}
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000039D4 File Offset: 0x00001BD4
		public static string _String(string input)
		{
			bool flag = string.IsNullOrEmpty(input);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				byte[] bytes = Encoding.ASCII.GetBytes(input);
				for (int i = 0; i < bytes.Length; i++)
				{
					byte[] array = bytes;
					int num = i;
					array[num] ^= 20;
				}
				result = Encoding.ASCII.GetString(bytes);
			}
			return result;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003A34 File Offset: 0x00001C34
		public static byte[] _Bytes(byte[] input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				int num = i;
				input[num] ^= 115;
			}
			return input;
		}
	}
}
