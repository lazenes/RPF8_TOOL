using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ModInstaller.ModCore
{
	// Token: 0x0200000F RID: 15
	public class FileModAnalyzer
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00003078 File Offset: 0x00001278
		public List<FileInfo> GetFilesInModFile(BinaryReader FileMod)
		{
			List<FileInfo> FilesInModFile = new List<FileInfo>();
			FileMod.BaseStream.Seek(0L, SeekOrigin.Begin);
			int FileCount = FileMod.ReadInt32();
			for (int i = 0; i < FileCount; i++)
			{
				FileInfo File = new FileInfo
				{
					FileCommand = (FileEnum)FileMod.ReadInt32(),
					FileName = MakeBackup._String(FileMod.ReadString()),
					MakeBackup = FileMod.ReadBoolean(),
					FilePath = MakeBackup._String(FileMod.ReadString()),
					FileSize = FileMod.ReadInt64(),
					FileOffet = FileMod.BaseStream.Position
				};
				FileMod.BaseStream.Seek(File.FileSize, SeekOrigin.Current);
				FilesInModFile.Add(File);
				//ProcessPage.progressDes.Report("جارٍ التحقق من صحة ملفات المثبت...");
				//ProcessPage.progress.Report((i + 1) * 100 / FileCount);
				Thread.Sleep(100);
			}
			return FilesInModFile;
		}
	}
}
