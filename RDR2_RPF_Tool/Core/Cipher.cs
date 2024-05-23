using System;
using System.Collections.Generic;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x02000018 RID: 24
	public static class Cipher
	{
		// Token: 0x0600016A RID: 362 RVA: 0x00006104 File Offset: 0x00004304
		public static ICipher GetCipher(int Tag, Platform platform)
		{
			bool flag = platform == Platform.Pc;
			ICipher result;
			if (flag)
			{
				Tfit2CbcCipher.Tfit2Key tfit2Keyvalue;
				bool flag2 = !KeysContainer.keysValues.TryGetValue(Tag, out tfit2Keyvalue);
				if (flag2)
				{
					throw new Exception("Can't get encoder key!");
				}
				result = new Tfit2CbcCipher(tfit2Keyvalue, KeysContainer.iv, KeysContainer.tfit2Context);
			}
			else
			{
				bool flag3 = platform == Platform.Ps4;
				if (!flag3)
				{
					throw new Exception("unknown platform!");
				}
				bool flag4 = Tag >= 163;
				if (flag4)
				{
					bool flag5 = Tag == 192;
					if (!flag5)
					{
						throw new Exception("Can't get encoder key!");
					}
					Tag = 163;
				}
				result = new TfitCbcCipher(KeysContainer.RDR2_PS4_TFIT_KEYS[Tag], KeysContainer.RDR2_PS4_TFIT_TABLES, KeysContainer.iv);
			}
			return result;
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000061B8 File Offset: 0x000043B8
		public static byte[] DecodeBlock(byte[] bytes, RPF8.Entry entry)
		{
			bool flag = entry.GetEncryptionKeyId() == byte.MaxValue;
			byte[] result;
			if (flag)
			{
				result = bytes;
			}
			else
			{
				int size = entry.GetOrignalSize();
				int raw_size = entry.GetOnDiskSize();
				bool is_compressed = entry.GetCompressorId() > Compressorid.None;
				bool isSignatureProtected = entry.IsSignatureProtected;
				if (isSignatureProtected)
				{
					raw_size -= 256;
				}
				bool isResource = entry.IsResource;
				if (isResource)
				{
					raw_size -= 16;
				}
				long chunk_size = (long)(entry.IsResource ? (is_compressed ? 524288 : size) : (is_compressed ? 8192 : 4096));
				List<List<long>> longs = StridedCipher.UnpackConfig(entry.GetEncryptionConfig(), (long)raw_size, chunk_size);
				ICipher tfit2CbcCipher = Cipher.GetCipher((int)entry.GetEncryptionKeyId(), entry.platform);
				foreach (List<long> BlockOffset in longs)
				{
					int start = (int)BlockOffset[0];
					int lenght = (int)(BlockOffset[1] - BlockOffset[0]);
					bytes = tfit2CbcCipher.Decode(bytes, new int?(start), new int?(lenght));
				}
				result = bytes;
			}
			return result;
		}
	}
}
