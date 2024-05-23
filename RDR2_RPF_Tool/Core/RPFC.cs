using System;
using System.Collections.Generic;
using System.IO;
using Helper;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x02000023 RID: 35
	public class RPFC
	{
		// Token: 0x06000190 RID: 400 RVA: 0x00037F1C File Offset: 0x0003611C
		public static RPFC Load(RPF8 rpf8)
		{
			RPFC rpfc = new RPFC();
			rpfc.LoadedFromRPF = true;
			rpfc.rpf8 = rpf8;
			rpfc.platform = rpf8.header.PlatformId;
			rpfc.FileKeyHash = RPFHelper.FileHash("0x25CF7830", rpfc.platform);
			rpfc.entry = rpf8.Entries[rpfc.FileKeyHash];
			rpfc.LoadFile(rpf8.GetFile(rpfc.FileKeyHash, true));
			return rpfc;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00037F98 File Offset: 0x00036198
		public static RPFC Load(string PfmFile)
		{
			RPFC rpfc = new RPFC();
			rpfc.LoadedFromRPF = false;
			rpfc.PFMFilePath = PfmFile;
			rpfc.platform = Platform.Ps4;
			rpfc.LoadFile(File.ReadAllBytes(PfmFile));
			return rpfc;
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00037FD4 File Offset: 0x000361D4
		public void LoadFile(byte[] bytes)
		{
			this.memorylist = new MStream(bytes);
			bool flag = this.memorylist.GetUIntValue(true, -1, Endian.Little) != 1128681554U;
			if (flag)
			{
				throw new Exception("Invalid 'pfm.dat' file!");
			}
			this.memorylist.Skip(4L);
			int Files = this.memorylist.GetIntValue(true, -1, Endian.Little);
			this.memorylist.Seek(44L, SeekOrigin.Begin);
			RPFC.FileEntry[] FilesEntries = new RPFC.FileEntry[Files];
			for (int i = 0; i < Files; i++)
			{
				this.memorylist.Skip(4L);
				FilesEntries[i].hash = this.memorylist.GetUIntValue(true, -1, Endian.Little);
				FilesEntries[i].Rpf8Size = this.memorylist.GetIntValue(true, -1, Endian.Little);
				FilesEntries[i].NameSize = this.memorylist.GetIntValue(true, -1, Endian.Little);
				int FileNameLenght = this.memorylist.GetIntValue(true, -1, Endian.Little);
				string FileName = this.memorylist.GetStringValue(FileNameLenght, true, -1, null).TrimEnd(new char[1]);
				FilesEntries[i].RpfName = this.FixedPath(FileName);
				int TableCount = this.memorylist.GetIntValue(true, -1, Endian.Little);
				this.memorylist.Skip((long)(TableCount * 8));
			}
			this.memorylist.Skip(4L);
			for (int j = 0; j < Files; j++)
			{
				FilesEntries[j].BlockOffset = this.memorylist.GetPosition();
				this.memorylist.Skip((long)FilesEntries[j].Rpf8Size);
				this.RPFCValue.Add(FilesEntries[j].RpfName, FilesEntries[j]);
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0003819C File Offset: 0x0003639C
		public void UpdateRpfBlock(string FileKey, byte[] Rpfbytes)
		{
			bool flag = !this.RPFCValue.ContainsKey(FileKey);
			if (flag)
			{
				Console.WriteLine("Can't find this entry: " + FileKey);
			}
			else
			{
				Console.WriteLine("entry: " + FileKey);
				RPFC.FileEntry fileEntry = this.RPFCValue[FileKey];
				this.memorylist.Seek(fileEntry.BlockOffset, SeekOrigin.Begin);
				this.memorylist.SetBytes(Rpfbytes, true, -1);
				bool loadedFromRPF = this.LoadedFromRPF;
				if (loadedFromRPF)
				{
					this.rpf8.ImportFile(this.FileKeyHash, this.memorylist.ToArray());
					this.rpf8.ReBuild();
				}
				else
				{
					this.memorylist.WriteFile(this.PFMFilePath);
				}
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00038260 File Offset: 0x00036460
		public string FixedPath(string path)
		{
			path = path.Replace("update:/", "");
			path = path.Replace("update_platform:/", "");
			bool flag = this.platform == Platform.Pc;
			if (flag)
			{
				path = path.Replace("platform:/", "x64/");
			}
			else
			{
				bool flag2 = this.platform == Platform.Ps4;
				if (flag2)
				{
					path = path.Replace("platform:/", "ps4/");
				}
			}
			return RPFHelper.FileHash(path, this.platform);
		}

		// Token: 0x0400006F RID: 111
		public RPF8 rpf8;

		// Token: 0x04000070 RID: 112
		public Platform platform;

		// Token: 0x04000071 RID: 113
		public RPF8.Entry entry;

		// Token: 0x04000072 RID: 114
		public string FileKeyHash;

		// Token: 0x04000073 RID: 115
		public string PFMFilePath;

		// Token: 0x04000074 RID: 116
		public MStream memorylist;

		// Token: 0x04000075 RID: 117
		public bool LoadedFromRPF;

		// Token: 0x04000076 RID: 118
		public Dictionary<string, RPFC.FileEntry> RPFCValue = new Dictionary<string, RPFC.FileEntry>();

		// Token: 0x02000039 RID: 57
		public struct FileEntry
		{
			// Token: 0x04000BF1 RID: 3057
			public uint hash;

			// Token: 0x04000BF2 RID: 3058
			public int Rpf8Size;

			// Token: 0x04000BF3 RID: 3059
			public int NameSize;

			// Token: 0x04000BF4 RID: 3060
			public int len;

			// Token: 0x04000BF5 RID: 3061
			public string RpfName;

			// Token: 0x04000BF6 RID: 3062
			public long BlockOffset;
		}
	}
}
