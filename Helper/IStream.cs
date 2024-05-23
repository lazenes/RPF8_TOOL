using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
	// Token: 0x02000016 RID: 22
	public interface IStream
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000B9 RID: 185
		bool CanRead { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000BA RID: 186
		bool CanWrite { get; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000BB RID: 187
		bool CanSeek { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000BC RID: 188
		long Length { get; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000BD RID: 189
		string Name { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000BE RID: 190
		// (set) Token: 0x060000BF RID: 191
		long Position { get; set; }

		// Token: 0x060000C0 RID: 192
		void Flush();

		// Token: 0x060000C1 RID: 193
		void SetLength(long value);

		// Token: 0x060000C2 RID: 194
		int Read(byte[] array, int offset, int count);

		// Token: 0x060000C3 RID: 195
		void Write(byte[] array, int offset, int count);

		// Token: 0x060000C4 RID: 196
		int ReadByte();

		// Token: 0x060000C5 RID: 197
		void WriteByte(byte value);

		// Token: 0x060000C6 RID: 198
		bool EndofFile();

		// Token: 0x060000C7 RID: 199
		void WriteFile(string path);

		// Token: 0x060000C8 RID: 200
		Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

		// Token: 0x060000C9 RID: 201
		Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

		// Token: 0x060000CA RID: 202
		Task FlushAsync(CancellationToken cancellationToken);

		// Token: 0x060000CB RID: 203
		void Close();

		// Token: 0x060000CC RID: 204
		void Dispose();

		// Token: 0x060000CD RID: 205
		void DeleteBoolValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000CE RID: 206
		void DeleteBytes(int Count, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000CF RID: 207
		void DeleteByteValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D0 RID: 208
		void DeleteDoubleValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D1 RID: 209
		void DeleteFloatValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D2 RID: 210
		void DeleteInt64Value(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D3 RID: 211
		void DeleteIntValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D4 RID: 212
		void DeleteShortValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D5 RID: 213
		void DeleteStringN(bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

		// Token: 0x060000D6 RID: 214
		void DeleteUByteValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D7 RID: 215
		void DeleteUInt64Value(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D8 RID: 216
		void DeleteUIntValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000D9 RID: 217
		void DeleteUShortValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000DA RID: 218
		T Get<T>(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000DB RID: 219
		T[] GetArray<T>(int Count, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000DC RID: 220
		bool GetBoolValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000DD RID: 221
		byte[] GetBytes(int Count, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000DE RID: 222
		byte GetByteValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000DF RID: 223
		double GetDoubleValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000E0 RID: 224
		float GetFloatValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000E1 RID: 225
		long GetInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000E2 RID: 226
		int GetIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000E3 RID: 227
		long GetPosition();

		// Token: 0x060000E4 RID: 228
		sbyte GetSByteValue(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000E5 RID: 229
		short GetShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000E6 RID: 230
		long GetSize();

		// Token: 0x060000E7 RID: 231
		string GetStringValue(int StringLenght, Encoding encoding);

		// Token: 0x060000E8 RID: 232
		string GetStringValue(int StringLenght, bool SavePosition, Encoding encoding);

		// Token: 0x060000E9 RID: 233
		string GetStringValue(int StringLenght, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

		// Token: 0x060000EA RID: 234
		string GetStringValueN(bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

		// Token: 0x060000EB RID: 235
		T GetStructureValues<T>(bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000EC RID: 236
		ulong GetUInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000ED RID: 237
		uint GetUIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000EE RID: 238
		ushort GetUShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000EF RID: 239
		void InsertBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000F0 RID: 240
		void InsertBytes(byte[] buffer, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000F1 RID: 241
		void InsertByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000F2 RID: 242
		void InsertDoubleValue(double value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000F3 RID: 243
		void InsertFloatValue(float value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000F4 RID: 244
		void InsertInt64Value(long value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000F5 RID: 245
		void InsertIntValue(int value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000F6 RID: 246
		void InsertShortValue(short value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000F7 RID: 247
		void InsertStringValue(string String, Encoding encoding);

		// Token: 0x060000F8 RID: 248
		void InsertStringValue(string String, bool SavePosition, Encoding encoding);

		// Token: 0x060000F9 RID: 249
		void InsertStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

		// Token: 0x060000FA RID: 250
		void InsertStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

		// Token: 0x060000FB RID: 251
		void InsertUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x060000FC RID: 252
		void InsertUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000FD RID: 253
		void InsertUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000FE RID: 254
		void InsertUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x060000FF RID: 255
		void ReplaceBytes(int OldBytesLenght, byte[] NewBytes, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x06000100 RID: 256
		long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin);

		// Token: 0x06000101 RID: 257
		void SetBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x06000102 RID: 258
		void SetBytes(byte[] buffer, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x06000103 RID: 259
		void SetByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x06000104 RID: 260
		void SetDoubleValue(double value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x06000105 RID: 261
		void SetFloatValue(float value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x06000106 RID: 262
		void SetInt64Value(long value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x06000107 RID: 263
		void SetIntValue(int value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x06000108 RID: 264
		void SetPosition(long offset);

		// Token: 0x06000109 RID: 265
		void SetShortValue(short value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x0600010A RID: 266
		void SetSize(long Size);

		// Token: 0x0600010B RID: 267
		void SetStringValue(string String, Encoding encoding);

		// Token: 0x0600010C RID: 268
		void SetStringValue(string String, bool SavePosition, Encoding encoding);

		// Token: 0x0600010D RID: 269
		void SetStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

		// Token: 0x0600010E RID: 270
		void SetStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

		// Token: 0x0600010F RID: 271
		void SetStructureValus<T>(T structure, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x06000110 RID: 272
		void SetUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1);

		// Token: 0x06000111 RID: 273
		void SetUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x06000112 RID: 274
		void SetUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x06000113 RID: 275
		void SetUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

		// Token: 0x06000114 RID: 276
		void Skip(long Count);

		// Token: 0x06000115 RID: 277
		byte[] ToArray();
	}
}
