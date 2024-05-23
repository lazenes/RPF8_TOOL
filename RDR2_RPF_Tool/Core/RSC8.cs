using System;
using System.Runtime.InteropServices;
using Helper;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x02000025 RID: 37
	public class RSC8
	{
		// Token: 0x0600019F RID: 415 RVA: 0x000387C0 File Offset: 0x000369C0
		public static void CreateRsc8(ref byte[] bytes, RPF8.Entry entry, bool UseEntry = false)
		{
			bool flag = !entry.IsResource;
			if (!flag)
			{
				RSC8.RSC8Info rsc8info = default(RSC8.RSC8Info);
				bool flag2 = !UseEntry;
				if (flag2)
				{
					rsc8info.Magic = RSC8.RSC8Magic;
					rsc8info.SetResourceId(entry.GetResourceType());
					rsc8info.SetCompressorId(entry.GetCompressorId());
					rsc8info.IsSignatureProtected = entry.IsSignatureProtected;
					rsc8info.SetEncryptionConfig(entry.GetEncryptionConfig());
					rsc8info.SetEncryptionKeyId(entry.GetEncryptionKeyId());
					rsc8info.Val2 = entry.Val3;
				}
				else
				{
					rsc8info.Magic = RSC8.RSC8Magic;
					rsc8info.SetResourceId(entry.GetResourceType());
					rsc8info.SetCompressorId(Compressorid.None);
					rsc8info.IsSignatureProtected = false;
					rsc8info.SetEncryptionConfig(0);
					rsc8info.SetEncryptionKeyId(byte.MaxValue);
					rsc8info.Val2 = entry.Val3;
				}
				bytes = RSC8.CreateHeader(bytes, rsc8info);
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x000388B3 File Offset: 0x00036AB3
		public static void CreateRsc8(ref byte[] bytes, RSC8.RSC8Info rsc8info)
		{
			bytes = RSC8.CreateHeader(bytes, rsc8info);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x000388C0 File Offset: 0x00036AC0
		private static byte[] CreateHeader(byte[] bytes, RSC8.RSC8Info rsc8info)
		{
			int structureSize = Marshal.SizeOf(typeof(RSC8.RSC8Info));
			byte[] buffer = new byte[structureSize];
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			Marshal.StructureToPtr<RSC8.RSC8Info>(rsc8info, handle.AddrOfPinnedObject(), false);
			handle.Free();
			byte[] TempArray = new byte[buffer.Length + bytes.Length];
			Buffer.BlockCopy(buffer, 0, TempArray, 0, buffer.Length);
			Buffer.BlockCopy(bytes, 0, TempArray, buffer.Length, bytes.Length);
			bytes = TempArray;
			return bytes;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00038938 File Offset: 0x00036B38
		public static RSC8.RSC8Info ReadRsc8(byte[] bytes)
		{
			RSC8.RSC8Info rsc8info = default(RSC8.RSC8Info);
			rsc8info.Magic = BitConverter.ToUInt32(bytes, 0);
			rsc8info.Val1 = BitConverter.ToUInt32(bytes, 4);
			rsc8info.Val2 = BitConverter.ToUInt64(bytes, 8);
			bool flag = rsc8info.Magic != RSC8.RSC8Magic;
			if (flag)
			{
				throw new Exception("Invalid resource header");
			}
			return rsc8info;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x000389A4 File Offset: 0x00036BA4
		public static RSC8.RSC8Info ReadRsc8(IStream memoryList)
		{
			RSC8.RSC8Info rsc8info = default(RSC8.RSC8Info);
			rsc8info.Magic = memoryList.GetUIntValue(false, 0, Endian.Little);
			rsc8info.Val1 = memoryList.GetUIntValue(false, 4, Endian.Little);
			rsc8info.Val2 = memoryList.GetUInt64Value(false, 8, Endian.Little);
			bool flag = rsc8info.Magic != RSC8.RSC8Magic;
			if (flag)
			{
				throw new Exception("Invalid resource header");
			}
			return rsc8info;
		}

		// Token: 0x04000079 RID: 121
		public static uint RSC8Magic = 943936338U;

		// Token: 0x0200003A RID: 58
		public struct RSC8Info
		{
			// Token: 0x1700001C RID: 28
			// (get) Token: 0x060001FC RID: 508 RVA: 0x0003A981 File Offset: 0x00038B81
			// (set) Token: 0x060001FD RID: 509 RVA: 0x0003A989 File Offset: 0x00038B89
			public uint Magic { get; set; }

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x060001FE RID: 510 RVA: 0x0003A992 File Offset: 0x00038B92
			// (set) Token: 0x060001FF RID: 511 RVA: 0x0003A99A File Offset: 0x00038B9A
			public uint Val1 { get; set; }

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x06000200 RID: 512 RVA: 0x0003A9A3 File Offset: 0x00038BA3
			// (set) Token: 0x06000201 RID: 513 RVA: 0x0003A9AB File Offset: 0x00038BAB
			public ulong Val2 { get; set; }

			// Token: 0x06000202 RID: 514 RVA: 0x0003A9B4 File Offset: 0x00038BB4
			public GetResourceType GetResourceId()
			{
				return (GetResourceType)(this.Val1 & 255U);
			}

			// Token: 0x06000203 RID: 515 RVA: 0x0003A9D3 File Offset: 0x00038BD3
			public void SetResourceId(GetResourceType ResourceId)
			{
				this.Val1 = this.Val1.ReplaceBits((uint)ResourceId, 0, 8);
			}

			// Token: 0x06000204 RID: 516 RVA: 0x0003A9EC File Offset: 0x00038BEC
			public Compressorid GetCompressorId()
			{
				byte compressorid = (byte)(this.Val1 >> 8 & 31U);
				bool flag = compressorid == 31;
				Compressorid result;
				if (flag)
				{
					result = Compressorid.None;
				}
				else
				{
					compressorid += 1;
					result = (Compressorid)compressorid;
				}
				return result;
			}

			// Token: 0x06000205 RID: 517 RVA: 0x0003AA20 File Offset: 0x00038C20
			public void SetCompressorId(Compressorid CompressorId)
			{
				this.Val1 = this.Val1.ReplaceBits((uint)(CompressorId - Compressorid.Deflate & 31), 8, 5);
			}

			// Token: 0x1700001F RID: 31
			// (get) Token: 0x06000206 RID: 518 RVA: 0x0003AA40 File Offset: 0x00038C40
			// (set) Token: 0x06000207 RID: 519 RVA: 0x0003AA61 File Offset: 0x00038C61
			public bool IsSignatureProtected
			{
				get
				{
					return (byte)(this.Val1 >> 13 & 7U) > 0;
				}
				set
				{
					this.Val1 = this.Val1.ReplaceBits(Convert.ToUInt32(value), 13, 3);
				}
			}

			// Token: 0x06000208 RID: 520 RVA: 0x0003AA80 File Offset: 0x00038C80
			public byte GetEncryptionKeyId()
			{
				return (byte)((this.Val1 >> 16 & 255U) - 1U);
			}

			// Token: 0x06000209 RID: 521 RVA: 0x0003AAA4 File Offset: 0x00038CA4
			public void SetEncryptionKeyId(byte EncryptionKeyId)
			{
				this.Val1 = this.Val1.ReplaceBits((uint)(EncryptionKeyId + 1), 16, 8);
			}

			// Token: 0x0600020A RID: 522 RVA: 0x0003AAC0 File Offset: 0x00038CC0
			public byte GetEncryptionConfig()
			{
				return (byte)(this.Val1 >> 24 & 255U);
			}

			// Token: 0x0600020B RID: 523 RVA: 0x0003AAE2 File Offset: 0x00038CE2
			public void SetEncryptionConfig(byte EncryptionConfig)
			{
				this.Val1 = this.Val1.ReplaceBits((uint)EncryptionConfig, 24, 8);
			}

			// Token: 0x0600020C RID: 524 RVA: 0x0003AAFB File Offset: 0x00038CFB
			public void SetOrignalSize(int Size)
			{
				this.Val2 = this.Val2.ReplaceBits((ulong)((long)Size), 0, 32);
			}
		}
	}
}
