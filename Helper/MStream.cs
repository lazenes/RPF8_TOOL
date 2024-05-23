using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Helper
{
	// Token: 0x02000017 RID: 23
	public class MStream : MemoryStream, IStream
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000116 RID: 278 RVA: 0x0000533E File Offset: 0x0000353E
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00005346 File Offset: 0x00003546
		public string Name { get; set; }

		// Token: 0x06000118 RID: 280 RVA: 0x0000534F File Offset: 0x0000354F
		public MStream()
		{
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000535C File Offset: 0x0000355C
		public MStream(string FilePath) : base(0)
		{
			byte[] bytes = File.ReadAllBytes(FilePath);
			this.Write(bytes, 0, bytes.Length);
			this.Name = FilePath;
			this.Position = 0L;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00005396 File Offset: 0x00003596
		public MStream(byte[] FileData) : base(0)
		{
			this.Write(FileData, 0, FileData.Length);
			this.Position = 0L;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000053B6 File Offset: 0x000035B6
		public MStream(string FileName, byte[] FileData) : base(FileData)
		{
			this.Name = FileName;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000053CC File Offset: 0x000035CC
		private T[] ConvertTo<T>(byte[] bytes, Endian endian = Endian.Little)
		{
			T[] array = new T[bytes.Length / Marshal.SizeOf(typeof(T))];
			for (int i = 0; i < array.Length; i++)
			{
				byte[] Val = new byte[Marshal.SizeOf(typeof(T))];
				Array.Copy(bytes, i * Marshal.SizeOf(typeof(T)), Val, 0, Marshal.SizeOf(typeof(T)));
				bool flag = endian == Endian.Big;
				if (flag)
				{
					Val.Reverse<byte>();
				}
				GCHandle handle = GCHandle.Alloc(Val, GCHandleType.Pinned);
				array[i] = (T)((object)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)));
				handle.Free();
			}
			return array;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000549C File Offset: 0x0000369C
		public T[] GetArray<T>(int Count, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return this.ConvertTo<T>(this.GetBytes(Count * Marshal.SizeOf(typeof(T)), SavePosition, SeekToOffset), endian);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000054D0 File Offset: 0x000036D0
		public T Get<T>(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return this.ConvertTo<T>(this.GetBytes(Marshal.SizeOf(typeof(T)), SavePosition, SeekToOffset), endian)[0];
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00005508 File Offset: 0x00003708
		public override long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
		{
			return base.Seek(offset, origin);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00005522 File Offset: 0x00003722
		public void Skip(long Count)
		{
			this.Seek(Count, SeekOrigin.Current);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00005530 File Offset: 0x00003730
		public long GetPosition()
		{
			return this.Position;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00005548 File Offset: 0x00003748
		public long GetSize()
		{
			return this.Length;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00005560 File Offset: 0x00003760
		public void SetPosition(long offset)
		{
			this.Position = offset;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000556B File Offset: 0x0000376B
		public void SetSize(long Size)
		{
			this.SetLength(Size);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00005578 File Offset: 0x00003778
		private void ExpandFile(long offset, int extraBytes)
		{
			byte[] buffer = new byte[4096];
			long length = this.Length;
			this.SetLength(length + (long)extraBytes);
			long pos = length;
			while (pos > offset)
			{
				int to_read = (pos - 4096L >= offset) ? 4096 : ((int)(pos - offset));
				pos -= (long)to_read;
				this.Position = pos;
				this.Read(buffer, 0, to_read);
				this.Position = pos + (long)extraBytes;
				this.Write(buffer, 0, to_read);
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x000055F8 File Offset: 0x000037F8
		private void ReduceFile(long offset, int extraBytes)
		{
			byte[] buffer = new byte[4096];
			long length = this.Length;
			int to_read;
			for (long pos = offset + (long)extraBytes; pos < length; pos += (long)to_read)
			{
				to_read = ((pos + 4096L <= length) ? 4096 : ((int)(length - pos)));
				this.Position = pos;
				this.Read(buffer, 0, to_read);
				this.Position = pos - (long)extraBytes;
				this.Write(buffer, 0, to_read);
			}
			this.SetLength(length - (long)extraBytes);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000567C File Offset: 0x0000387C
		public bool EndofFile()
		{
			return this.Position == this.Length;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000569C File Offset: 0x0000389C
		public void WriteFile(string path)
		{
			File.WriteAllBytes(path, this.ToArray());
		}

		// Token: 0x06000129 RID: 297 RVA: 0x000056AC File Offset: 0x000038AC
		public bool GetBoolValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			return Convert.ToBoolean(this.GetByteValue(SavePosition, SeekToOffset));
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000056CB File Offset: 0x000038CB
		public void SetBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.SetByteValue(Convert.ToByte(Value), SavePosition, SeekToOffset);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000056DD File Offset: 0x000038DD
		public void InsertBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.InsertByteValue(Convert.ToByte(Value), SavePosition, SeekToOffset);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000056EF File Offset: 0x000038EF
		public void DeleteBoolValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteByteValue(SavePosition, SeekToOffset);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000056FC File Offset: 0x000038FC
		public byte[] GetBytes(int Count, bool SavePosition = true, int SeekToOffset = -1)
		{
			long PositionBackup = this.Position;
			bool flag = SeekToOffset != -1;
			if (flag)
			{
				this.Seek((long)SeekToOffset, SeekOrigin.Begin);
			}
			byte[] buffer = new byte[Count];
			this.Read(buffer, 0, Count);
			bool flag2 = !SavePosition;
			if (flag2)
			{
				this.Seek(PositionBackup, SeekOrigin.Begin);
			}
			return buffer;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00005754 File Offset: 0x00003954
		public void SetBytes(byte[] buffer, bool SavePosition = true, int SeekToOffset = -1)
		{
			long PositionBackup = this.Position;
			bool flag = SeekToOffset != -1;
			if (flag)
			{
				this.Seek((long)SeekToOffset, SeekOrigin.Begin);
			}
			this.Write(buffer, 0, buffer.Length);
			bool flag2 = !SavePosition;
			if (flag2)
			{
				this.Seek(PositionBackup, SeekOrigin.Begin);
			}
			this.Flush();
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000057A8 File Offset: 0x000039A8
		public void InsertBytes(byte[] buffer, bool SavePosition = true, int SeekToOffset = -1)
		{
			long PositionBackup = this.Position;
			bool flag = SeekToOffset != -1;
			if (flag)
			{
				this.Seek((long)SeekToOffset, SeekOrigin.Begin);
			}
			long NewPosition = this.Position;
			this.ExpandFile(NewPosition, buffer.Length);
			this.Position = NewPosition;
			this.SetBytes(buffer, true, -1);
			bool flag2 = !SavePosition;
			if (flag2)
			{
				this.Seek(PositionBackup, SeekOrigin.Begin);
			}
			this.Flush();
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00005814 File Offset: 0x00003A14
		public void DeleteBytes(int Count, bool SavePosition = true, int SeekToOffset = -1)
		{
			long PositionBackup = this.Position;
			bool flag = SeekToOffset != -1;
			if (flag)
			{
				this.Seek((long)SeekToOffset, SeekOrigin.Begin);
			}
			long NewPosition = this.Position;
			this.ReduceFile(NewPosition, Count);
			this.Seek(NewPosition, SeekOrigin.Begin);
			bool flag2 = !SavePosition;
			if (flag2)
			{
				this.Seek(PositionBackup, SeekOrigin.Begin);
			}
			this.Flush();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00005873 File Offset: 0x00003A73
		public void ReplaceBytes(int OldBytesLenght, byte[] NewBytes, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(OldBytesLenght, SavePosition, SeekToOffset);
			this.InsertBytes(NewBytes, SavePosition, SeekToOffset);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000588C File Offset: 0x00003A8C
		public byte GetByteValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			return this.GetBytes(1, SavePosition, SeekToOffset)[0];
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000058AC File Offset: 0x00003AAC
		public sbyte GetSByteValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			return (sbyte)this.GetBytes(1, SavePosition, SeekToOffset)[0];
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000058CA File Offset: 0x00003ACA
		public void SetByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.SetBytes(new byte[]
			{
				value
			}, SavePosition, SeekToOffset);
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000058E0 File Offset: 0x00003AE0
		public void SetUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.SetBytes(new byte[]
			{
				(byte)value
			}, SavePosition, SeekToOffset);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x000058F7 File Offset: 0x00003AF7
		public void InsertByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.InsertBytes(new byte[]
			{
				value
			}, SavePosition, SeekToOffset);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000590D File Offset: 0x00003B0D
		public void InsertUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.InsertBytes(new byte[]
			{
				(byte)value
			}, SavePosition, SeekToOffset);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00005924 File Offset: 0x00003B24
		public void DeleteByteValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(1, SavePosition, SeekToOffset);
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00005931 File Offset: 0x00003B31
		public void DeleteUByteValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(1, SavePosition, SeekToOffset);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00005940 File Offset: 0x00003B40
		public short GetShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = this.GetBytes(2, SavePosition, SeekToOffset);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToInt16(array, 0);
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00005974 File Offset: 0x00003B74
		public ushort GetUShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return (ushort)this.GetShortValue(SavePosition, SeekToOffset, endian);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00005990 File Offset: 0x00003B90
		public void SetShortValue(short value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.SetBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000059C1 File Offset: 0x00003BC1
		public void SetUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.SetShortValue((short)value, SavePosition, SeekToOffset, endian);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000059D4 File Offset: 0x00003BD4
		public void InsertShortValue(short value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.InsertBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00005A05 File Offset: 0x00003C05
		public void InsertUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.InsertShortValue((short)value, SavePosition, SeekToOffset, endian);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00005A15 File Offset: 0x00003C15
		public void DeleteShortValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(2, SavePosition, SeekToOffset);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00005A22 File Offset: 0x00003C22
		public void DeleteUShortValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteShortValue(SavePosition, SeekToOffset);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00005A30 File Offset: 0x00003C30
		public int GetIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = this.GetBytes(4, SavePosition, SeekToOffset);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToInt32(array, 0);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00005A64 File Offset: 0x00003C64
		public uint GetUIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return (uint)this.GetIntValue(SavePosition, SeekToOffset, endian);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00005A80 File Offset: 0x00003C80
		public void SetIntValue(int value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.SetBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00005AB1 File Offset: 0x00003CB1
		public void SetUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.SetIntValue((int)value, SavePosition, SeekToOffset, endian);
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00005AC0 File Offset: 0x00003CC0
		public void InsertIntValue(int value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.InsertBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00005AF1 File Offset: 0x00003CF1
		public void InsertUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.InsertIntValue((int)value, SavePosition, SeekToOffset, endian);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00005B00 File Offset: 0x00003D00
		public void DeleteIntValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(4, SavePosition, SeekToOffset);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00005B0D File Offset: 0x00003D0D
		public void DeleteUIntValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteIntValue(SavePosition, SeekToOffset);
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00005B1C File Offset: 0x00003D1C
		public long GetInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = this.GetBytes(8, SavePosition, SeekToOffset);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToInt64(array, 0);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00005B50 File Offset: 0x00003D50
		public ulong GetUInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return (ulong)this.GetInt64Value(SavePosition, SeekToOffset, endian);
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00005B6C File Offset: 0x00003D6C
		public void SetInt64Value(long value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.SetBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00005B9D File Offset: 0x00003D9D
		public void SetUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.SetInt64Value((long)value, SavePosition, SeekToOffset, endian);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00005BAC File Offset: 0x00003DAC
		public void InsertInt64Value(long value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.InsertBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00005BDD File Offset: 0x00003DDD
		public void InsertUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.InsertInt64Value((long)value, SavePosition, SeekToOffset, endian);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00005BEC File Offset: 0x00003DEC
		public void DeleteInt64Value(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(8, SavePosition, SeekToOffset);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00005BF9 File Offset: 0x00003DF9
		public void DeleteUInt64Value(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteInt64Value(SavePosition, SeekToOffset);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00005C08 File Offset: 0x00003E08
		public double GetDoubleValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = this.GetBytes(8, SavePosition, SeekToOffset);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToDouble(array, 0);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00005C3C File Offset: 0x00003E3C
		public void SetDoubleValue(double value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.SetBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00005C70 File Offset: 0x00003E70
		public void InsertDoubleValue(double value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.InsertBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00005CA1 File Offset: 0x00003EA1
		public void DeleteDoubleValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(8, SavePosition, SeekToOffset);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00005CB0 File Offset: 0x00003EB0
		public float GetFloatValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = this.GetBytes(4, SavePosition, SeekToOffset);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToSingle(array, 0);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00005CE4 File Offset: 0x00003EE4
		public void SetFloatValue(float value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.SetBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00005D18 File Offset: 0x00003F18
		public void InsertFloatValue(float value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			byte[] array = BitConverter.GetBytes(value);
			bool flag = endian == Endian.Big;
			if (flag)
			{
				Array.Reverse(array);
			}
			this.InsertBytes(array, SavePosition, SeekToOffset);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00005D49 File Offset: 0x00003F49
		public void DeleteFloatValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(4, SavePosition, SeekToOffset);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00005D56 File Offset: 0x00003F56
		public string GetStringValue(int StringLenght, Encoding encoding)
		{
			return this.GetStringValue(StringLenght, true, -1, encoding);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00005D62 File Offset: 0x00003F62
		public string GetStringValue(int StringLenght, bool SavePosition, Encoding encoding)
		{
			return this.GetStringValue(StringLenght, SavePosition, -1, encoding);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00005D70 File Offset: 0x00003F70
		public string GetStringValue(int StringLenght, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			byte[] array = this.GetBytes(StringLenght, SavePosition, SeekToOffset);
			return encoding.GetString(array);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00005DA6 File Offset: 0x00003FA6
		public void SetStringValue(string String, Encoding encoding)
		{
			this.SetStringValue(String, true, -1, encoding);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00005DB3 File Offset: 0x00003FB3
		public void SetStringValue(string String, bool SavePosition, Encoding encoding)
		{
			this.SetStringValue(String, SavePosition, -1, encoding);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00005DC0 File Offset: 0x00003FC0
		public void SetStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			this.SetBytes(encoding.GetBytes(String), SavePosition, SeekToOffset);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00005DF1 File Offset: 0x00003FF1
		public void InsertStringValue(string String, Encoding encoding)
		{
			this.InsertStringValue(String, true, -1, encoding);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00005DFE File Offset: 0x00003FFE
		public void InsertStringValue(string String, bool SavePosition, Encoding encoding)
		{
			this.InsertStringValue(String, SavePosition, -1, encoding);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00005E0C File Offset: 0x0000400C
		public void InsertStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			this.InsertBytes(encoding.GetBytes(String), SavePosition, SeekToOffset);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00005E40 File Offset: 0x00004040
		private byte[] GetString(Encoding encoding)
		{
			List<byte> StringValues = new List<byte>();
			bool flag = encoding != Encoding.Unicode;
			if (flag)
			{
				bool flag2;
				do
				{
					StringValues.Add(this.GetByteValue(true, -1));
					flag2 = (StringValues[StringValues.Count - 1] == 0);
				}
				while (!flag2);
			}
			else
			{
				bool flag3;
				do
				{
					StringValues.Add(this.GetByteValue(true, -1));
					StringValues.Add(this.GetByteValue(true, -1));
					flag3 = (StringValues[StringValues.Count - 1] == 0 && StringValues[StringValues.Count - 2] == 0);
				}
				while (!flag3);
			}
			return StringValues.ToArray();
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00005EF8 File Offset: 0x000040F8
		public string GetStringValueN(bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			long Position = this.Position;
			bool flag2 = SeekToOffset != -1;
			if (flag2)
			{
				this.Seek((long)SeekToOffset, SeekOrigin.Begin);
			}
			string Value = encoding.GetString(this.GetString(encoding)).TrimEnd(new char[1]);
			bool flag3 = !SavePosition;
			if (flag3)
			{
				this.Seek(Position, SeekOrigin.Begin);
			}
			return Value;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00005F6C File Offset: 0x0000416C
		public void DeleteStringN(bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			long Position = this.Position;
			bool flag2 = SeekToOffset != -1;
			if (flag2)
			{
				this.Seek((long)SeekToOffset, SeekOrigin.Begin);
			}
			this.DeleteBytes(this.GetString(encoding).Length, SavePosition, SeekToOffset);
			this.Seek(Position, SeekOrigin.Begin);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00005FC4 File Offset: 0x000041C4
		public void SetStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			this.SetBytes(encoding.GetBytes(String + "\0"), SavePosition, SeekToOffset);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00006000 File Offset: 0x00004200
		public void InsertStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			this.InsertBytes(encoding.GetBytes(String + "\0"), SavePosition, SeekToOffset);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000603C File Offset: 0x0000423C
		public T GetStructureValues<T>(bool SavePosition = true, int SeekToOffset = -1)
		{
			int structureSize = Marshal.SizeOf(typeof(T));
			byte[] buffer = this.GetBytes(structureSize, SavePosition, SeekToOffset);
			bool flag = buffer.Length != structureSize;
			if (flag)
			{
				throw new Exception("could not read all of data for structure");
			}
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			T structure = (T)((object)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)));
			handle.Free();
			return structure;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x000060B4 File Offset: 0x000042B4
		public void SetStructureValus<T>(T structure, bool SavePosition = true, int SeekToOffset = -1)
		{
			int structureSize = Marshal.SizeOf(typeof(T));
			byte[] buffer = new byte[structureSize];
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			Marshal.StructureToPtr<T>(structure, handle.AddrOfPinnedObject(), false);
			handle.Free();
			this.SetBytes(buffer, SavePosition, SeekToOffset);
		}
	}
}
