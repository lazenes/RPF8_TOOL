using System;

namespace ModInstaller.ModCore
{
	public class FileInfo
	{
		
		public FileEnum FileCommand { get; set; }		
		public string FileName { get; set; }		
		public bool MakeBackup { get; set; }		
		public string FilePath { get; set; }		
		public long FileOffet { get; set; }		
		public long FileSize { get; set; }
		public string RpfPath { get; set; }
	}
}
