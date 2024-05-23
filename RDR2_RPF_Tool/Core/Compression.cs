using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x02000019 RID: 25
	internal class Compression
	{
		// Token: 0x0600016C RID: 364
		[DllImport("oo2core_5_win64.dll")]
		internal static extern int OodleLZ_Decompress(byte[] Buffer, long BufferSize, byte[] OutputBuffer, long OutputBufferSize, uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h, uint i, int ThreadModule);

		// Token: 0x0600016D RID: 365
		[DllImport("oo2core_5_win64.dll")]
		internal static extern int OodleLZ_Compress(Compression.OodleFormat Format, byte[] Buffer, long BufferSize, byte[] OutputBuffer, Compression.OodleCompressionLevel Level, uint a, uint b, uint c);

		// Token: 0x0600016E RID: 366 RVA: 0x000062EC File Offset: 0x000044EC
		public static byte[] OodleLZ_Decompress(byte[] data, int decompressedSize)
		{
			byte[] decompressedData = new byte[decompressedSize];
			uint verificationSize = (uint)Compression.OodleLZ_Decompress(data, (long)data.Length, decompressedData, (long)decompressedData.Length, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 3);
			bool flag = (ulong)verificationSize != (ulong)((long)decompressedSize);
			if (flag)
			{
				throw new Exception("Decompression failed. Verification size does not match given size.");
			}
			return decompressedData;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000633C File Offset: 0x0000453C
		public static byte[] OodleLZ_Compress(byte[] data, Compression.OodleFormat format = Compression.OodleFormat.Kraken, Compression.OodleCompressionLevel level = Compression.OodleCompressionLevel.Normal)
		{
			byte[] compressedData = new byte[data.Length];
			int verificationSize = Compression.OodleLZ_Compress(format, data, (long)data.Length, compressedData, level, 0U, 0U, 0U);
			bool flag = verificationSize == 0;
			if (flag)
			{
				throw new Exception("Compression failed. Verification size is 0.");
			}
			return compressedData;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00006380 File Offset: 0x00004580
		public static byte[] DeflateDecompress(byte[] data)
		{
			byte[] decompressedArray = null;
			using (MemoryStream decompressedStream = new MemoryStream())
			{
				using (MemoryStream compressStream = new MemoryStream(data))
				{
					using (DeflateStream deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress))
					{
						deflateStream.CopyTo(decompressedStream);
					}
				}
				decompressedArray = decompressedStream.ToArray();
			}
			return decompressedArray;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00006410 File Offset: 0x00004610
		public static byte[] DeflateCompress(byte[] data)
		{
			byte[] compressArray = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
				{
					deflateStream.Write(data, 0, data.Length);
				}
				compressArray = memoryStream.ToArray();
			}
			return compressArray;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00006480 File Offset: 0x00004680
		public static byte[] DecompressFile(byte[] bytes, int usize, Compressorid compressorid)
		{
			byte[] result;
			switch (compressorid)
			{
			case Compressorid.None:
				Array.Resize<byte>(ref bytes, usize);
				result = bytes;
				break;
			case Compressorid.Deflate:
				result = Compression.DeflateDecompress(bytes);
				break;
			case Compressorid.Oodle:
				result = Compression.OodleLZ_Decompress(bytes, usize);
				break;
			default:
				throw new Exception("unkown compression type: " + compressorid.ToString());
			}
			return result;
		}

		// Token: 0x02000032 RID: 50
		internal enum OodleFormat : uint
		{
			// Token: 0x04000BC6 RID: 3014
			LZH,
			// Token: 0x04000BC7 RID: 3015
			LZHLW,
			// Token: 0x04000BC8 RID: 3016
			LZNIB,
			// Token: 0x04000BC9 RID: 3017
			None,
			// Token: 0x04000BCA RID: 3018
			LZB16,
			// Token: 0x04000BCB RID: 3019
			LZBLW,
			// Token: 0x04000BCC RID: 3020
			LZA,
			// Token: 0x04000BCD RID: 3021
			LZNA,
			// Token: 0x04000BCE RID: 3022
			Kraken,
			// Token: 0x04000BCF RID: 3023
			Mermaid,
			// Token: 0x04000BD0 RID: 3024
			BitKnit,
			// Token: 0x04000BD1 RID: 3025
			Selkie,
			// Token: 0x04000BD2 RID: 3026
			Hydra,
			// Token: 0x04000BD3 RID: 3027
			Leviathan
		}

		// Token: 0x02000033 RID: 51
		internal enum OodleCompressionLevel : uint
		{
			// Token: 0x04000BD5 RID: 3029
			None,
			// Token: 0x04000BD6 RID: 3030
			SuperFast,
			// Token: 0x04000BD7 RID: 3031
			VeryFast,
			// Token: 0x04000BD8 RID: 3032
			Fast,
			// Token: 0x04000BD9 RID: 3033
			Normal,
			// Token: 0x04000BDA RID: 3034
			Optimal1,
			// Token: 0x04000BDB RID: 3035
			Optimal2,
			// Token: 0x04000BDC RID: 3036
			Optimal3,
			// Token: 0x04000BDD RID: 3037
			Optimal4,
			// Token: 0x04000BDE RID: 3038
			Optimal5
		}
	}
}
