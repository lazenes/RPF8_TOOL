using System;
using System.Linq;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x02000028 RID: 40
	public class TfitCbcCipher : ICipher
	{
		// Token: 0x060001B4 RID: 436 RVA: 0x00039AA4 File Offset: 0x00037CA4
		private static void TFIT_DecryptRoundA(byte[] data, uint[] key, uint[][] table)
		{
			uint[] temp = new uint[]
			{
				table[0][(int)data[0]] ^ table[1][(int)data[1]] ^ table[2][(int)data[2]] ^ table[3][(int)data[3]] ^ key[0],
				table[4][(int)data[4]] ^ table[5][(int)data[5]] ^ table[6][(int)data[6]] ^ table[7][(int)data[7]] ^ key[1],
				table[8][(int)data[8]] ^ table[9][(int)data[9]] ^ table[10][(int)data[10]] ^ table[11][(int)data[11]] ^ key[2],
				table[12][(int)data[12]] ^ table[13][(int)data[13]] ^ table[14][(int)data[14]] ^ table[15][(int)data[15]] ^ key[3]
			};
			Buffer.BlockCopy(temp, 0, data, 0, 16);
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00039B6C File Offset: 0x00037D6C
		private static void TFIT_DecryptRoundB(byte[] data, uint[] key, uint[][] table)
		{
			uint[] temp = new uint[]
			{
				table[0][(int)data[0]] ^ table[1][(int)data[7]] ^ table[2][(int)data[10]] ^ table[3][(int)data[13]] ^ key[0],
				table[4][(int)data[1]] ^ table[5][(int)data[4]] ^ table[6][(int)data[11]] ^ table[7][(int)data[14]] ^ key[1],
				table[8][(int)data[2]] ^ table[9][(int)data[5]] ^ table[10][(int)data[8]] ^ table[11][(int)data[15]] ^ key[2],
				table[12][(int)data[3]] ^ table[13][(int)data[6]] ^ table[14][(int)data[9]] ^ table[15][(int)data[12]] ^ key[3]
			};
			Buffer.BlockCopy(temp, 0, data, 0, 16);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00039C34 File Offset: 0x00037E34
		private static void TFIT_DecryptBlock(byte[] input, byte[] output, uint[][] keys, uint[][][] tables)
		{
			byte[] temp = new byte[16];
			Array.Copy(input, 0, temp, 0, 16);
			TfitCbcCipher.TFIT_DecryptRoundA(temp, keys[0], tables[0]);
			TfitCbcCipher.TFIT_DecryptRoundA(temp, keys[1], tables[1]);
			for (int i = 2; i < 16; i++)
			{
				TfitCbcCipher.TFIT_DecryptRoundB(temp, keys[i], tables[i]);
			}
			TfitCbcCipher.TFIT_DecryptRoundA(temp, keys[16], tables[16]);
			Buffer.BlockCopy(temp, 0, output, 0, 16);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00039CA9 File Offset: 0x00037EA9
		public TfitCbcCipher(uint[][] keys, uint[][][] tables, byte[] iv)
		{
			this.iv_ = new byte[iv.Length];
			Array.Copy(iv, this.iv_, 16);
			this.keys_ = keys.Skip(1).ToArray<uint[]>();
			this.tables_ = tables;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00039CEC File Offset: 0x00037EEC
		public byte[] Decode(byte[] input, int? start = null, int? lenght = null)
		{
			bool flag = start == null && lenght == null;
			if (flag)
			{
				start = new int?(0);
				lenght = new int?(input.Length);
			}
			int StartPosition = start.Value;
			int BlockSize = lenght.Value;
			int Size = BlockSize - BlockSize % 16;
			byte[] Cur_iv = this.iv_;
			for (int i = StartPosition; i < StartPosition + Size; i += 16)
			{
				byte[] bytes = new byte[16];
				byte[] Next_iv = new byte[16];
				Buffer.BlockCopy(input, i, Next_iv, 0, 16);
				byte[] outbytes = new byte[16];
				Buffer.BlockCopy(input, i, bytes, 0, 16);
				TfitCbcCipher.TFIT_DecryptBlock(bytes, outbytes, this.keys_, this.tables_);
				for (int j = 0; j < 16; j++)
				{
					//input[i + j] = (outbytes[j] ^ Cur_iv[j]);
                    input[j] = (byte)(outbytes[j] ^ Cur_iv[j]);
                }
				Buffer.BlockCopy(Next_iv, 0, Cur_iv, 0, 16);
			}
			Buffer.BlockCopy(Cur_iv, 0, this.iv_, 0, 16);
			return input;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00039E04 File Offset: 0x00038004
		public static byte[] TFIT_DecryptBytes(uint[][] keys, uint[][][] tables, byte[] iv, byte[] input, int? start = null, int? lenght = null)
		{
			TfitCbcCipher tfit2CbcCipher = new TfitCbcCipher(keys, tables, iv);
			return tfit2CbcCipher.Decode(input, start, lenght);
		}

		// Token: 0x0400007E RID: 126
		public uint[][] keys_;

		// Token: 0x0400007F RID: 127
		public uint[][][] tables_;

		// Token: 0x04000080 RID: 128
		public byte[] iv_;

		// Token: 0x04000081 RID: 129
		private const int TFIT_BLOCK_SIZE = 16;
	}
}
