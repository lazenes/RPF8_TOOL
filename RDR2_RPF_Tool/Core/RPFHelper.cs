using System;
using System.Collections;
using System.IO;
using System.Linq;
using Helper;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x02000024 RID: 36
	internal static class RPFHelper
	{
		// Token: 0x06000196 RID: 406 RVA: 0x000382FC File Offset: 0x000364FC
		public static int SetPadding(this IStream fStream, byte PaddingByte = 0)
		{
			int padding = 0;
			bool flag = fStream.Length % 16L != 0L;
			if (flag)
			{
				padding = (int)(16L - fStream.Length % 16L);
				fStream.SetBytes(Enumerable.Repeat<byte>(PaddingByte, padding).ToArray<byte>(), true, -1);
			}
			return padding;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0003834C File Offset: 0x0003654C
		private static byte[] ConvertToArray(this BitArray bitArray)
		{
			byte[] array = new byte[bitArray.Length / 8];
			bitArray.CopyTo(array, 0);
			return array;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00038378 File Offset: 0x00036578
		public static ulong ReplaceBits(this ulong input, ulong value, int pos, int BitSize)
		{
			BitArray bitArray = new BitArray(BitConverter.GetBytes(input));
			BitArray bitArray2 = new BitArray(BitConverter.GetBytes(value));
			for (int i = 0; i < BitSize; i++)
			{
				bitArray[pos + i] = bitArray2[i];
			}
			return BitConverter.ToUInt64(bitArray.ConvertToArray(), 0);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x000383D4 File Offset: 0x000365D4
		public static uint ReplaceBits(this uint input, uint value, int pos, int BitSize)
		{
			BitArray bitArray = new BitArray(BitConverter.GetBytes(input));
			BitArray bitArray2 = new BitArray(BitConverter.GetBytes(value));
			for (int i = 0; i < BitSize; i++)
			{
				bitArray[pos + i] = bitArray2[i];
			}
			return BitConverter.ToUInt32(bitArray.ConvertToArray(), 0);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00038430 File Offset: 0x00036630
		public static string FileHash(string path, Platform platform)
		{
			int Fileid = RPFHelper.GetFileExtId(Path.GetExtension(path), platform);
			string FilePath = path.Replace("\\", "/");
			bool flag = FilePath.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
			string result;
			if (flag)
			{
				FilePath = RPFHelper.GetFileName(Convert.ToUInt32(Path.GetFileNameWithoutExtension(path), 16));
				result = FilePath + RPFHelper.GetFileExt(Fileid, platform);
			}
			else
			{
				bool flag2 = Fileid != 255;
				if (flag2)
				{
					result = RPFHelper.GetFileName(JOAATHash.Calc(Path.ChangeExtension(path, null))) + RPFHelper.GetFileExt(Fileid, platform);
				}
				else
				{
					result = RPFHelper.GetFileName(JOAATHash.Calc(path)) + RPFHelper.GetFileExt(Fileid, platform);
				}
			}
			return result;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x000384E0 File Offset: 0x000366E0
		public static int GetFileExtId(string ext, Platform platform)
		{
			ext = ext.TrimStart(new char[]
			{
				'.'
			}).Trim();
			for (int i = 0; i < RPFHelper.BaseRageExts.Length; i++)
			{
				bool flag = RPFHelper.BaseRageExts[i].Replace('#', (char)platform) == ext;
				if (flag)
				{
					return i;
				}
			}
			for (int j = 0; j < RPFHelper.ExtraRageExts.Length; j++)
			{
				bool flag2 = RPFHelper.ExtraRageExts[j].Replace('#', (char)platform) == ext;
				if (flag2)
				{
					return j + 64;
				}
			}
			return 255;
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00038588 File Offset: 0x00036788
		public static string GetFileExt(int id, Platform platform)
		{
			bool flag = id >= 0 && id < RPFHelper.BaseRageExts.Length;
			string result;
			if (flag)
			{
				result = "." + RPFHelper.BaseRageExts[id].Replace('#', (char)platform);
			}
			else
			{
				bool flag2 = id >= 64 && id <= 87;
				if (flag2)
				{
					result = "." + RPFHelper.ExtraRageExts[id - 64].Replace('#', (char)platform);
				}
				else
				{
					result = "";
				}
			}
			return result;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00038608 File Offset: 0x00036808
		public static string GetFileName(uint Hash)
		{
			bool flag = NamesContainer.Names.ContainsKey(Hash);
			string result;
			if (flag)
			{
				result = NamesContainer.Names[Hash];
			}
			else
			{
				result = "0x" + Hash.ToString("x2").ToUpper();
			}
			return result;
		}

		// Token: 0x04000077 RID: 119
		private static string[] BaseRageExts = new string[]
		{
			"rpf",
			"#mf",
			"#dr",
			"#ft",
			"#dd",
			"#td",
			"#bn",
			"#bd",
			"#pd",
			"#bs",
			"#sd",
			"#mt",
			"#sc",
			"#cs"
		};

		// Token: 0x04000078 RID: 120
		private static string[] ExtraRageExts = new string[]
		{
			"mrf",
			"cut",
			"gfx",
			"#cd",
			"#ld",
			"#pmd",
			"#pm",
			"#ed",
			"#pt",
			"#map",
			"#typ",
			"#ch",
			"#ldb",
			"#jd",
			"#ad",
			"#nv",
			"#hn",
			"#pl",
			"#nd",
			"#vr",
			"#wr",
			"#nh",
			"#fd",
			"#as"
		};
	}
}
