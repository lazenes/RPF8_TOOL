using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Helper;

namespace RDR2_RPF_Tool.Core
{
	public class RPF8
	{
		public static RPF8 Load(string path)
		{
			RPF8 rpf8 = new RPF8();
			rpf8.Rpf8StreamFile = FStream.Open(path, FileMode.Open, FileAccess.ReadWrite);
			rpf8.FilePath = path;
			rpf8.Load();
			rpf8.FilePath = Path.GetFullPath(rpf8.FilePath).Replace(Path.GetFullPath(Path.GetDirectoryName(rpf8.FilePath)), "").TrimStart(new char[]
			{
				'\\'
			});
			return rpf8;
		}

		public static RPF8 Load(string VirtualPath, byte[] bytes)
		{
			string TempPath = Path.Combine(new string[]
			{
				Path.GetTempPath(),
				Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title,
				Path.GetDirectoryName(VirtualPath),
				DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"),
				Path.GetFileName(VirtualPath)
			});
			bool flag = !Directory.Exists(Path.GetDirectoryName(TempPath));
			if (flag)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(TempPath));
			}
			File.WriteAllBytes(TempPath, bytes);
			RPF8 rpf8 = RPF8.Load(TempPath);
			rpf8.IsTemp = true;
			rpf8.FilePath = VirtualPath;
			return rpf8;
		}

		public void Load()
		{
			this.header = this.Rpf8StreamFile.GetStructureValues<RPF8.Rpf8Header>(true, -1);
			bool flag = this.header.Magic != this.RPF8MAGIC;
			if (flag)
			{
				this.Rpf8StreamFile.Close();
				throw new Exception("This file '" + this.FilePath + "' is not rpf8 format!");
			}
			this.rsa_signature = this.Rpf8StreamFile.GetBytes(256, true, -1);
			bool flag2 = this.header.DecryptionTag != 255;
			MStream memoryList;
			if (flag2)
			{
				memoryList = new MStream(Cipher.GetCipher((int)this.header.DecryptionTag, this.header.PlatformId).Decode(this.Rpf8StreamFile.GetBytes(this.header.EntryCount * 24, true, -1), null, null));
			}
			else
			{
				memoryList = new MStream(this.Rpf8StreamFile.GetBytes(this.header.EntryCount * 24, true, -1));
			}
			for (int i = 0; i < this.header.EntryCount; i++)
			{
				RPF8.Entry entry = new RPF8.Entry();
				entry.platform = this.header.PlatformId;
				entry.Val1 = memoryList.GetUInt64Value(true, -1, Endian.Little);
				entry.Val2 = memoryList.GetUInt64Value(true, -1, Endian.Little);
				entry.Val3 = memoryList.GetUInt64Value(true, -1, Endian.Little);
				this.Entries.Add(entry.GetName(), entry);
			}
			this.Rpf8StreamFile.Seek(this.Rpf8StreamFile.GetSize() - (long)this.header.NamesLength, SeekOrigin.Begin);
			this.Names = this.Rpf8StreamFile.GetBytes(this.header.NamesLength, true, -1);
			Console.WriteLine("FilePath: " + this.FilePath);
		}

		
		public void Destroy()
		{
			bool flag = this.Rpf8StreamFile != null;
			if (flag)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Distroy File: " + this.Rpf8StreamFile.Name);
				Console.ResetColor();
				string path = this.Rpf8StreamFile.Name;
				this.Rpf8StreamFile.Close();
				this.Rpf8StreamFile.Dispose();
				this.Rpf8StreamFile = null;
				this.Entries.Clear();
				this.header = default(RPF8.Rpf8Header);
				this.rsa_signature = null;
				this.Names = null;
				bool isTemp = this.IsTemp;
				if (isTemp)
				{
					File.Delete(path);
				}
			}
		}

		
		~RPF8()
		{
			this.Destroy();
		}

		
		private void UpdateHeader(IStream fStream)
		{
			long oldPosition = fStream.Position;
			fStream.Position = 0L;
			this.header.DecryptionTag = 255;
			fStream.SetStructureValus<RPF8.Rpf8Header>(this.header, true, -1);
			fStream.SetBytes(this.rsa_signature, true, -1);
			foreach (KeyValuePair<string, RPF8.Entry> entry in this.Entries)
			{
				fStream.SetUInt64Value(entry.Value.Val1, true, -1, Endian.Little);
				fStream.SetUInt64Value(entry.Value.Val2, true, -1, Endian.Little);
				fStream.SetUInt64Value(entry.Value.Val3, true, -1, Endian.Little);
			}
			bool flag = this.RPFCFile != null && !this.IsTemp;
			if (flag)
			{
				int HeaderSize = (int)fStream.GetPosition();
				fStream.Position = 0L;
				byte[] Header = fStream.GetBytes(HeaderSize, true, -1);
				Console.WriteLine("This is the path: " + this.FilePath);
				this.RPFCFile.UpdateRpfBlock(RPFHelper.FileHash(this.FilePath, this.header.PlatformId), Header);
			}
			fStream.Position = oldPosition;
		}

		
		public void ReBuild()
		{
			IStream fStream = new FStream(this.Rpf8StreamFile.Name + ".temp.rpf", FileMode.Create, FileAccess.ReadWrite);
			this.UpdateHeader(fStream);
			fStream.Seek((long)(272 + 24 * this.Entries.Count), SeekOrigin.Begin);
			fStream.SetPadding(0);
			for (int i = 0; i < this.Entries.Count; i++)
			{
				KeyValuePair<string, RPF8.Entry> entry = this.Entries.ElementAt(i);
				RPF8.Entry FileEntrie = entry.Value;
				this.Rpf8StreamFile.Seek(FileEntrie.GetOffset(), SeekOrigin.Begin);
				FileEntrie.SetOffset(fStream.Position);
				fStream.SetBytes(this.Rpf8StreamFile.GetBytes(FileEntrie.GetOnDiskSize(), true, -1), true, -1);
				fStream.SetPadding(0);
				this.Entries[entry.Key] = FileEntrie;
			}
			fStream.SetBytes(this.Names, true, -1);
			this.UpdateHeader(fStream);
			fStream.Close();
			this.Rpf8StreamFile.Close();
			File.Delete(this.Rpf8StreamFile.Name);
			File.Move(this.Rpf8StreamFile.Name + ".temp.rpf", this.Rpf8StreamFile.Name);
			this.Rpf8StreamFile = FStream.Open(this.Rpf8StreamFile.Name, FileMode.Open, FileAccess.ReadWrite);
		}

		
		public void ImportFiles(string FileMap)
		{
			this.ImportFiles(File.ReadAllLines(FileMap), Path.GetDirectoryName(FileMap));
		}

		
		public void ImportFiles(string[] FileList, string Directory)
		{
			string[] Files = (from x in FileList
			where !string.IsNullOrWhiteSpace(x)
			select x).ToArray<string>();
			foreach (string FilePath in Files)
			{
				string path = Path.Combine(Directory, FilePath);
				bool flag = !File.Exists(path);
				if (flag)
				{
					throw new FileNotFoundException("Can't find this file in directory: " + path);
				}
			}
			foreach (string filePath in Files)
			{
				string Hash = RPFHelper.FileHash(filePath, this.header.PlatformId);
				bool flag2 = !this.Entries.ContainsKey(Hash);
				if (flag2)
				{
					Console.WriteLine("Can't find this file '{0}'", filePath);
				}
				else
				{
					string FilePath2 = Path.Combine(Directory, filePath);
					byte[] FileBytes = File.ReadAllBytes(FilePath2);
					this.ImportFile(Hash, FileBytes);
				}
			}
		}

		
		public void ImportFiles(string[] Entrykeys, byte[][] FilesBytes)
		{
			bool flag = FilesBytes.Length != Entrykeys.Length;
			if (flag)
			{
				throw new Exception("Entrykeys and FilesBytes size do not match");
			}
			for (int i = 0; i < Entrykeys.Length; i++)
			{
				bool isResource = this.Entries[Entrykeys[i]].IsResource;
				if (isResource)
				{
					RSC8.ReadRsc8(FilesBytes[i]);
				}
			}
			this.Rpf8StreamFile.SetLength(this.Rpf8StreamFile.GetSize() - (long)this.header.NamesLength);
			this.Rpf8StreamFile.Seek(this.Rpf8StreamFile.Length, SeekOrigin.Begin);
			this.Rpf8StreamFile.SetPadding(0);
			for (int j = 0; j < Entrykeys.Length; j++)
			{
				RPF8.Entry entry = this.Entries[Entrykeys[j]];
				entry.SetOffset(this.Rpf8StreamFile.Length);
				int Size = FilesBytes[j].Length;
				Compressorid compressorid = this.CompressFile(ref FilesBytes[j], entry);
				this.Rpf8StreamFile.SetBytes(FilesBytes[j], true, -1);
				int padding = this.Rpf8StreamFile.SetPadding(0);
				bool flag2 = !entry.IsResource;
				if (flag2)
				{
					entry.SetEncryptionConfig(0);
					entry.SetEncryptionKeyId(byte.MaxValue);
					entry.SetCompressorId(compressorid);
					entry.SetOnDiskSize(padding + FilesBytes[j].Length);
					entry.SetOrignalSize(Size);
				}
				else
				{
					RSC8.RSC8Info Header = RSC8.ReadRsc8(FilesBytes[j]);
					entry.SetEncryptionConfig(Header.GetEncryptionConfig());
					entry.SetEncryptionKeyId(Header.GetEncryptionKeyId());
					entry.SetCompressorId(Header.GetCompressorId());
					entry.SetOnDiskSize(padding + FilesBytes[j].Length);
					entry.Val3 = Header.Val2;
				}
				this.Entries[Entrykeys[j]] = entry;
				bool flag3 = Entrykeys[j].EndsWith(".rpf", StringComparison.OrdinalIgnoreCase);
				if (flag3)
				{
					this.RPFCFile.UpdateRpfBlock(RPFHelper.FileHash(Entrykeys[j], this.header.PlatformId), FilesBytes[j].ToList<byte>().GetRange(0, 272 + 24 * BitConverter.ToInt32(FilesBytes[j], 4)).ToArray());
				}
			}
			this.UpdateHeader(this.Rpf8StreamFile);
			this.Rpf8StreamFile.Seek(this.Rpf8StreamFile.Length, SeekOrigin.Begin);
			this.Rpf8StreamFile.SetBytes(this.Names, true, -1);
		}

		
		private Compressorid CompressFile(ref byte[] FileBytes, RPF8.Entry entry)
		{
			bool flag = entry.GetFileExtId() == 0;
			Compressorid result;
			if (flag)
			{
				result = Compressorid.None;
			}
			else
			{
				bool flag2 = !entry.IsResource;
				if (flag2)
				{
					FileBytes = Compression.DeflateCompress(FileBytes);
				}
				else
				{
					RSC8.RSC8Info Header = RSC8.ReadRsc8(FileBytes);
					bool flag3 = Header.GetCompressorId() == Compressorid.None;
					if (flag3)
					{
						FileBytes = FileBytes.Skip(16).ToArray<byte>();
						FileBytes = Compression.DeflateCompress(FileBytes);
						Header.SetCompressorId(Compressorid.Deflate);
						RSC8.CreateRsc8(ref FileBytes, Header);
					}
				}
				result = Compressorid.Deflate;
			}
			return result;
		}

		
		public void ImportFile(string EntryKey, byte[] FileBytes)
		{
			Console.WriteLine("Entry key: " + EntryKey);
			RPF8.Entry entry = this.Entries[EntryKey];
			bool isResource = entry.IsResource;
			if (isResource)
			{
				RSC8.ReadRsc8(FileBytes);
			}
			this.Rpf8StreamFile.SetLength(this.Rpf8StreamFile.GetSize() - (long)this.header.NamesLength);
			this.Rpf8StreamFile.Seek(this.Rpf8StreamFile.Length, SeekOrigin.Begin);
			this.Rpf8StreamFile.SetPadding(0);
			entry.SetOffset(this.Rpf8StreamFile.Length);
			int Size = FileBytes.Length;
			Compressorid compressorid = this.CompressFile(ref FileBytes, entry);
			this.Rpf8StreamFile.SetBytes(FileBytes, true, -1);
			int padding = this.Rpf8StreamFile.SetPadding(0);
			bool flag = !entry.IsResource;
			if (flag)
			{
				entry.SetEncryptionConfig(0);
				entry.SetEncryptionKeyId(byte.MaxValue);
				entry.SetCompressorId(compressorid);
				entry.SetOnDiskSize(padding + FileBytes.Length);
				entry.SetOrignalSize(Size);
			}
			else
			{
				RSC8.RSC8Info Header = RSC8.ReadRsc8(FileBytes);
				entry.SetEncryptionConfig(Header.GetEncryptionConfig());
				entry.SetEncryptionKeyId(Header.GetEncryptionKeyId());
				entry.SetCompressorId(Header.GetCompressorId());
				entry.SetOnDiskSize(padding + FileBytes.Length);
				entry.Val3 = Header.Val2;
			}
			this.Entries[EntryKey] = entry;
			this.UpdateHeader(this.Rpf8StreamFile);
			this.Rpf8StreamFile.Seek(this.Rpf8StreamFile.Length, SeekOrigin.Begin);
			this.Rpf8StreamFile.SetBytes(this.Names, true, -1);
			bool flag2 = EntryKey.EndsWith(".rpf", StringComparison.OrdinalIgnoreCase);
			if (flag2)
			{
				this.RPFCFile.UpdateRpfBlock(RPFHelper.FileHash(EntryKey, this.header.PlatformId), FileBytes.ToList<byte>().GetRange(0, 272 + 24 * BitConverter.ToInt32(FileBytes, 4)).ToArray());
			}
		}

		
		public byte[] GetFile(string hash, bool DecodeResourceFile = true)
		{
			RPF8.Entry entry = this.Entries[hash];
			int raw_size = entry.GetOnDiskSize();
			long offset = entry.GetOffset();
			bool isSignatureProtected = entry.IsSignatureProtected;
			if (isSignatureProtected)
			{
				bool flag = raw_size < 256;
				if (flag)
				{
					throw new Exception("Signature protected file is too small");
				}
				raw_size -= 256;
				offset += 256L;
			}
			bool isResource = entry.IsResource;
			if (isResource)
			{
				bool flag2 = raw_size < 16;
				if (flag2)
				{
					throw new Exception("Resource raw size is too small");
				}
				offset += 16L;
				raw_size -= 16;
			}
			this.Rpf8StreamFile.Seek(offset, SeekOrigin.Begin);
			byte[] Filebytes = this.Rpf8StreamFile.GetBytes(raw_size, true, -1);
			bool flag3 = (entry.IsResource && DecodeResourceFile) || !entry.IsResource;
			if (flag3)
			{
				Filebytes = Cipher.DecodeBlock(Filebytes, entry);
				Filebytes = Compression.DecompressFile(Filebytes, entry.GetOrignalSize(), entry.GetCompressorId());
			}
			RSC8.CreateRsc8(ref Filebytes, entry, DecodeResourceFile);
			return Filebytes;
		}

		
		public readonly uint RPF8MAGIC = 1380992568U;

		
		public string FilePath;

	
		public RPFC RPFCFile;

		
		public IStream Rpf8StreamFile;

		
		public RPF8.Rpf8Header header;

		
		public byte[] rsa_signature;


		public Dictionary<string, RPF8.Entry> Entries = new Dictionary<string, RPF8.Entry>();

	
		public byte[] Names;

	
		public bool IsTemp = false;

		
		public struct Rpf8Header
		{
	
			public uint Magic;

		
			public int EntryCount;

			public int NamesLength;

		
			public ushort DecryptionTag;

			[MarshalAs(UnmanagedType.U2)]
			public Platform PlatformId;
		}

	
		public class Entry
		{
			
			public ulong Val1 { get; set; }

			public ulong Val2 { get; set; }

		
			public ulong Val3 { get; set; }

			public string GetName()
			{
				return RPFHelper.GetFileName(this.GetHash()) + RPFHelper.GetFileExt((int)this.GetFileExtId(), this.platform);
			}

			
			public string GetAttributes()
			{
				string attributes = "";
				bool isResource = this.IsResource;
				if (isResource)
				{
					attributes = string.Format("Resourced [Virsion: {0}];", (int)this.GetResourceType());
				}
				bool flag = this.GetCompressorId() > Compressorid.None;
				if (flag)
				{
					attributes += string.Format("Compressed [{0}];", this.GetCompressorId());
				}
				bool flag2 = this.GetEncryptionKeyId() != byte.MaxValue;
				if (flag2)
				{
					attributes += "Encrypted;";
				}
				bool flag3 = string.IsNullOrEmpty(attributes);
				if (flag3)
				{
					attributes = "No";
				}
				return attributes;
			}

			public uint GetHash()
			{
				return (uint)this.Val1;
			}

		
			public void SetHash(uint Hash)
			{
				this.Val1 = this.Val1.ReplaceBits((ulong)Hash, 0, 32);
			}

		
			public byte GetEncryptionConfig()
			{
				return (byte)(this.Val1 >> 32 & 255UL);
			}

		
			public void SetEncryptionConfig(byte EncryptionConfig)
			{
				this.Val1 = this.Val1.ReplaceBits((ulong)EncryptionConfig, 32, 8);
			}

		
			public byte GetEncryptionKeyId()
			{
				return (byte)(this.Val1 >> 40 & 255UL);
			}

		
			public void SetEncryptionKeyId(byte EncryptionKeyId)
			{
				this.Val1 = this.Val1.ReplaceBits((ulong)EncryptionKeyId, 40, 8);
			}

			
			public byte GetFileExtId()
			{
				return (byte)(this.Val1 >> 48 & 255UL);
			}

		
			public void SetFileExtId(byte FileExtId)
			{
				this.Val1 = this.Val1.ReplaceBits((ulong)FileExtId, 48, 8);
			}

			
			public bool IsResource
			{
				get
				{
					return Convert.ToBoolean((byte)(this.Val1 >> 56 & 1UL));
				}
				set
				{
					this.Val1 = this.Val1.ReplaceBits((ulong)Convert.ToByte(value), 56, 1);
				}
			}

			
			public bool IsSignatureProtected
			{
				get
				{
					return Convert.ToBoolean((byte)(this.Val1 >> 57 & 1UL));
				}
				set
				{
					this.Val1 = this.Val1.ReplaceBits((ulong)Convert.ToByte(value), 56, 1);
				}
			}

			
			public int GetOnDiskSize()
			{
				return (int)(this.Val2 & 268435455UL) << 4;
			}

			
			public void SetOnDiskSize(int OnDiskSize)
			{
				this.Val2 = this.Val2.ReplaceBits((ulong)((long)(OnDiskSize >> 4)), 0, 28);
			}

		
			public long GetOffset()
			{
				return (long)((long)(this.Val2 >> 28 & 2147483647UL) << 4);
			}

			public void SetOffset(long Offset)
			{
				this.Val2 = this.Val2.ReplaceBits((ulong)(Offset >> 4), 28, 31);
			}

			
			public Compressorid GetCompressorId()
			{
				return (Compressorid)(this.Val2 >> 59 & 31UL);
			}

			
			public void SetCompressorId(Compressorid compressorid)
			{
				this.Val2 = this.Val2.ReplaceBits((ulong)compressorid, 59, 5);
			}

			
			public int GetOrignalSize()
			{
				bool flag = !this.IsResource;
				int result;
				if (flag)
				{
					result = (int)this.Val3;
				}
				else
				{
                    unchecked
                    {
                        result = (int)(this.Val3 & (ulong)-16) + (int)(this.Val3 >> 32 & (ulong)-16);
				}
                }
                return result;
			}

			
			public void SetOrignalSize(int Size)
			{
				this.Val3 = this.Val3.ReplaceBits((ulong)((long)Size), 0, 32);
			}

		
			public GetResourceType GetResourceType()
			{
				return (GetResourceType)(this.Val3 >> 32 & 15UL);
			}

			public Platform platform;
		}
	}
}
