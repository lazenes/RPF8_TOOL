using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace Helper
{
	
	public class FStream : FileStream, IStream
	{
		
		public FStream(string path, FileMode mode, FileAccess access) : base(path, mode, access)
		{
		}

	
		public FStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
		{
		}

		
		public FStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : base(path, mode, access, share, bufferSize)
		{
		}

	
		public FStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) : base(path, mode, access, share, bufferSize, options)
		{
		}

		
		public FStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
		{
		}

	
		public FStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity) : base(path, mode, rights, share, bufferSize, options, fileSecurity)
		{
		}

		public static FStream Open(string path, FileMode mode, FileAccess access)
		{
			return new FStream(path, mode, access);
		}

	
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

	
		public T[] GetArray<T>(int Count, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return this.ConvertTo<T>(this.GetBytes(Count * Marshal.SizeOf(typeof(T)), SavePosition, SeekToOffset), endian);
		}

		
		public T Get<T>(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return this.ConvertTo<T>(this.GetBytes(Marshal.SizeOf(typeof(T)), SavePosition, SeekToOffset), endian)[0];
		}

		
		public override long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
		{
			return base.Seek(offset, origin);
		}

		
		public void Skip(long Count)
		{
			this.Seek(Count, SeekOrigin.Current);
		}

	
		public long GetPosition()
		{
			return this.Position;
		}

		
		public long GetSize()
		{
			return this.Length;
		}

		public void SetPosition(long offset)
		{
			this.Position = offset;
		}

		public void SetSize(long Size)
		{
			this.SetLength(Size);
		}

		
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

		
		public byte[] ToArray()
		{
			return this.GetBytes(Convert.ToInt32(this.Length), false, 0);
		}

	
		public bool EndofFile()
		{
			return this.Position == this.Length;
		}

		
		public void WriteFile(string path)
		{
			File.WriteAllBytes(path, this.ToArray());
		}

	
		public bool GetBoolValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			return Convert.ToBoolean(this.GetByteValue(SavePosition, SeekToOffset));
		}
	
		public void SetBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.SetByteValue(Convert.ToByte(Value), SavePosition, SeekToOffset);
		}

		
		public void InsertBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.InsertByteValue(Convert.ToByte(Value), SavePosition, SeekToOffset);
		}

		public void DeleteBoolValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteByteValue(SavePosition, SeekToOffset);
		}

		
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

		
		public void ReplaceBytes(int OldBytesLenght, byte[] NewBytes, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(OldBytesLenght, SavePosition, SeekToOffset);
			this.InsertBytes(NewBytes, SavePosition, SeekToOffset);
		}

		
		public byte GetByteValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			return this.GetBytes(1, SavePosition, SeekToOffset)[0];
		}

		
		public sbyte GetSByteValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			return (sbyte)this.GetBytes(1, SavePosition, SeekToOffset)[0];
		}

		
		public void SetByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.SetBytes(new byte[]
			{
				value
			}, SavePosition, SeekToOffset);
		}

		
		public void SetUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.SetBytes(new byte[]
			{
				(byte)value
			}, SavePosition, SeekToOffset);
		}

	
		public void InsertByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.InsertBytes(new byte[]
			{
				value
			}, SavePosition, SeekToOffset);
		}

		
		public void InsertUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1)
		{
			this.InsertBytes(new byte[]
			{
				(byte)value
			}, SavePosition, SeekToOffset);
		}

		
		public void DeleteByteValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(1, SavePosition, SeekToOffset);
		}

	
		public void DeleteUByteValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(1, SavePosition, SeekToOffset);
		}

		
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

	
		public ushort GetUShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return (ushort)this.GetShortValue(SavePosition, SeekToOffset, endian);
		}

	
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

		
		public void SetUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.SetShortValue((short)value, SavePosition, SeekToOffset, endian);
		}

		
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

		
		public void InsertUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.InsertShortValue((short)value, SavePosition, SeekToOffset, endian);
		}

		
		public void DeleteShortValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(2, SavePosition, SeekToOffset);
		}

		
		public void DeleteUShortValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteShortValue(SavePosition, SeekToOffset);
		}


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

		
		public uint GetUIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return (uint)this.GetIntValue(SavePosition, SeekToOffset, endian);
		}

		
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

		
		public void SetUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.SetIntValue((int)value, SavePosition, SeekToOffset, endian);
		}

	
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

		
		public void InsertUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.InsertIntValue((int)value, SavePosition, SeekToOffset, endian);
		}

		
		public void DeleteIntValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(4, SavePosition, SeekToOffset);
		}

		
		public void DeleteUIntValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteIntValue(SavePosition, SeekToOffset);
		}

		
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

		
		public ulong GetUInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			return (ulong)this.GetInt64Value(SavePosition, SeekToOffset, endian);
		}

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

		public void SetUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.SetInt64Value((long)value, SavePosition, SeekToOffset, endian);
		}

	
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

		
		public void InsertUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
		{
			this.InsertInt64Value((long)value, SavePosition, SeekToOffset, endian);
		}

		
		public void DeleteInt64Value(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(8, SavePosition, SeekToOffset);
		}

	
		public void DeleteUInt64Value(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteInt64Value(SavePosition, SeekToOffset);
		}

		
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

		// Token: 0x060000A3 RID: 163 RVA: 0x00004ED5 File Offset: 0x000030D5
		public void DeleteDoubleValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(8, SavePosition, SeekToOffset);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00004EE4 File Offset: 0x000030E4
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

		// Token: 0x060000A5 RID: 165 RVA: 0x00004F18 File Offset: 0x00003118
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

		// Token: 0x060000A6 RID: 166 RVA: 0x00004F4C File Offset: 0x0000314C
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

		// Token: 0x060000A7 RID: 167 RVA: 0x00004F7D File Offset: 0x0000317D
		public void DeleteFloatValue(bool SavePosition = true, int SeekToOffset = -1)
		{
			this.DeleteBytes(4, SavePosition, SeekToOffset);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004F8A File Offset: 0x0000318A
		public string GetStringValue(int StringLenght, Encoding encoding)
		{
			return this.GetStringValue(StringLenght, true, -1, encoding);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004F96 File Offset: 0x00003196
		public string GetStringValue(int StringLenght, bool SavePosition, Encoding encoding)
		{
			return this.GetStringValue(StringLenght, SavePosition, -1, encoding);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004FA4 File Offset: 0x000031A4
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

		// Token: 0x060000AB RID: 171 RVA: 0x00004FDA File Offset: 0x000031DA
		public void SetStringValue(string String, Encoding encoding)
		{
			this.SetStringValue(String, true, -1, encoding);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004FE7 File Offset: 0x000031E7
		public void SetStringValue(string String, bool SavePosition, Encoding encoding)
		{
			this.SetStringValue(String, SavePosition, -1, encoding);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004FF4 File Offset: 0x000031F4
		public void SetStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			this.SetBytes(encoding.GetBytes(String), SavePosition, SeekToOffset);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005025 File Offset: 0x00003225
		public void InsertStringValue(string String, Encoding encoding)
		{
			this.InsertStringValue(String, true, -1, encoding);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005032 File Offset: 0x00003232
		public void InsertStringValue(string String, bool SavePosition, Encoding encoding)
		{
			this.InsertStringValue(String, SavePosition, -1, encoding);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005040 File Offset: 0x00003240
		public void InsertStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			this.InsertBytes(encoding.GetBytes(String), SavePosition, SeekToOffset);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00005074 File Offset: 0x00003274
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

		// Token: 0x060000B2 RID: 178 RVA: 0x0000512C File Offset: 0x0000332C
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

		// Token: 0x060000B3 RID: 179 RVA: 0x000051A0 File Offset: 0x000033A0
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

		// Token: 0x060000B4 RID: 180 RVA: 0x000051F8 File Offset: 0x000033F8
		public void SetStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			this.SetBytes(encoding.GetBytes(String + "\0"), SavePosition, SeekToOffset);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005234 File Offset: 0x00003434
		public void InsertStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
		{
			bool flag = encoding == null;
			if (flag)
			{
				encoding = Encoding.ASCII;
			}
			this.InsertBytes(encoding.GetBytes(String + "\0"), SavePosition, SeekToOffset);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005270 File Offset: 0x00003470
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

		// Token: 0x060000B7 RID: 183 RVA: 0x000052E8 File Offset: 0x000034E8
		public void SetStructureValus<T>(T structure, bool SavePosition = true, int SeekToOffset = -1)
		{
			int structureSize = Marshal.SizeOf(typeof(T));
			byte[] buffer = new byte[structureSize];
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			Marshal.StructureToPtr<T>(structure, handle.AddrOfPinnedObject(), false);
			handle.Free();
			this.SetBytes(buffer, SavePosition, SeekToOffset);
		}

        // Token: 0x060000B8 RID: 184 RVA: 0x00005336 File Offset: 0x00003536
        string IStream.Name
        {
            get { return base.Name; }
        }
    }
}
