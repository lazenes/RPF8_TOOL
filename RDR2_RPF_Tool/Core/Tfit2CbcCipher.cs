using System;
using System.Collections.Generic;
using System.Linq;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x02000027 RID: 39
	public class Tfit2CbcCipher : ICipher
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x00038BA8 File Offset: 0x00036DA8
		private static void TFIT2_DecryptSplatBytes(byte[] bytes, ulong[] output)
		{
			for (int i = 0; i < 8; i++)
			{
				output[i] = 72340172838076673UL * (ulong)bytes[7 - i];
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00038BDC File Offset: 0x00036DDC
		private static ulong TFIT2_DecryptRoundBlock(ulong[] input1, ulong[] masks, uint xorr, ulong[] lookup)
		{
			ulong lower = (masks[15] & input1[7]) ^ (masks[14] & input1[6]) ^ (masks[13] & input1[5]) ^ (masks[12] & input1[4]) ^ (masks[11] & input1[3]) ^ (masks[10] & input1[2]) ^ (masks[9] & input1[1]) ^ (masks[8] & input1[0]);
			lower ^= lower >> 1;
			lower ^= lower >> 2;
			lower ^= lower >> 4;
			lower &= 72340172838076673UL;
			lower |= lower >> 7;
			lower |= lower >> 14;
			lower |= lower >> 28;
			ulong upper = (masks[7] & input1[7]) ^ (masks[6] & input1[6]) ^ (masks[5] & input1[5]) ^ (masks[4] & input1[4]) ^ (masks[3] & input1[3]) ^ (masks[2] & input1[2]) ^ (masks[1] & input1[1]) ^ (masks[0] & input1[0]);
			upper ^= upper << 1;
			upper ^= upper >> 2;
			upper ^= upper >> 4;
			upper &= 144680345676153346UL;
			upper |= upper << 7;
			upper |= upper >> 14;
			return lookup[(int)(checked((IntPtr)((lower & 255UL) ^ (upper & 3840UL) ^ unchecked((ulong)xorr))))];
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00038CEC File Offset: 0x00036EEC
		private static void TFIT2_DecryptRoundA(Tfit2CbcCipher.Tfit2Context ctx, byte[] data)
		{
			ulong[] values = new ulong[]
			{
				ctx.InitTables[0][(int)data[0]] ^ ctx.InitTables[1][(int)data[1]] ^ ctx.InitTables[2][(int)data[2]] ^ ctx.InitTables[3][(int)data[3]] ^ ctx.InitTables[4][(int)data[4]] ^ ctx.InitTables[5][(int)data[5]] ^ ctx.InitTables[6][(int)data[6]] ^ ctx.InitTables[7][(int)data[7]],
				ctx.InitTables[8][(int)data[8]] ^ ctx.InitTables[9][(int)data[9]] ^ ctx.InitTables[10][(int)data[10]] ^ ctx.InitTables[11][(int)data[11]] ^ ctx.InitTables[12][(int)data[12]] ^ ctx.InitTables[13][(int)data[13]] ^ ctx.InitTables[14][(int)data[14]] ^ ctx.InitTables[15][(int)data[15]]
			};
			Buffer.BlockCopy(values, 0, data, 0, 16);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00038E00 File Offset: 0x00037000
		private static void TFIT2_DecryptRoundB(Tfit2CbcCipher.Tfit2Context ctx, int index, byte[] data, ulong[] key)
		{
			Tfit2CbcCipher.Round round = ctx.Rounds[index];
			ulong[] v0 = new ulong[8];
			ulong[] v = new ulong[8];
			List<byte> dataList = data.ToList<byte>();
			Tfit2CbcCipher.TFIT2_DecryptSplatBytes(dataList.GetRange(0, 8).ToArray(), v0);
			Tfit2CbcCipher.TFIT2_DecryptSplatBytes(dataList.GetRange(8, 8).ToArray(), v);
			ulong[] val = new ulong[]
			{
				key[0] ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[0].Masks, round.Blocks[0].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[1].Masks, round.Blocks[1].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[2].Masks, round.Blocks[2].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[3].Masks, round.Blocks[3].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[4].Masks, round.Blocks[4].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[5].Masks, round.Blocks[5].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[6].Masks, round.Blocks[6].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[7].Masks, round.Blocks[7].Xor, round.Lookup),
				key[1] ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[8].Masks, round.Blocks[8].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[9].Masks, round.Blocks[9].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[10].Masks, round.Blocks[10].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[11].Masks, round.Blocks[11].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[12].Masks, round.Blocks[12].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[13].Masks, round.Blocks[13].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[14].Masks, round.Blocks[14].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[15].Masks, round.Blocks[15].Xor, round.Lookup)
			};
			Buffer.BlockCopy(val, 0, data, 0, 16);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x000391A8 File Offset: 0x000373A8
		private static void TFIT2_DecryptRoundC(Tfit2CbcCipher.Tfit2Context ctx, int index, byte[] data, ulong[] key)
		{
			Tfit2CbcCipher.Round round = ctx.Rounds[index];
			ulong[] v0 = new ulong[8];
			ulong[] v = new ulong[8];
			List<byte> dataList = data.ToList<byte>();
			Tfit2CbcCipher.TFIT2_DecryptSplatBytes(dataList.GetRange(0, 8).ToArray(), v0);
			Tfit2CbcCipher.TFIT2_DecryptSplatBytes(dataList.GetRange(8, 8).ToArray(), v);
			ulong[] val = new ulong[]
			{
				key[0] ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[0].Masks, round.Blocks[0].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[1].Masks, round.Blocks[1].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[2].Masks, round.Blocks[2].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[3].Masks, round.Blocks[3].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[4].Masks, round.Blocks[4].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[5].Masks, round.Blocks[5].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[6].Masks, round.Blocks[6].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[7].Masks, round.Blocks[7].Xor, round.Lookup),
				key[1] ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[8].Masks, round.Blocks[8].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[9].Masks, round.Blocks[9].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[10].Masks, round.Blocks[10].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[11].Masks, round.Blocks[11].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[12].Masks, round.Blocks[12].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v, round.Blocks[13].Masks, round.Blocks[13].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[14].Masks, round.Blocks[14].Xor, round.Lookup) ^ Tfit2CbcCipher.TFIT2_DecryptRoundBlock(v0, round.Blocks[15].Masks, round.Blocks[15].Xor, round.Lookup)
			};
			Buffer.BlockCopy(val, 0, data, 0, 16);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00039550 File Offset: 0x00037750
		private static byte TFIT2_DecryptSquashBytes(ulong[] input, ulong[] lookup)
		{
			ulong v = (lookup[7] & input[7]) ^ (lookup[6] & input[6]) ^ (lookup[5] & input[5]) ^ (lookup[4] & input[4]) ^ (lookup[3] & input[3]) ^ (lookup[2] & input[2]) ^ (lookup[1] & input[1]) ^ (lookup[0] & input[0]);
			v ^= v >> 1;
			v ^= v >> 2;
			v ^= v >> 4;
			v &= 72340172838076673UL;
			v |= v >> 7;
			v |= v >> 14;
			v |= v >> 28;
			return (byte)v;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x000395D8 File Offset: 0x000377D8
		private static void TFIT2_DecryptRoundD(Tfit2CbcCipher.Tfit2Context ctx, byte[] data)
		{
			ulong[] v0 = new ulong[8];
			ulong[] v = new ulong[8];
			List<byte> dataList = data.ToList<byte>();
			Tfit2CbcCipher.TFIT2_DecryptSplatBytes(dataList.GetRange(0, 8).ToArray(), v0);
			Tfit2CbcCipher.TFIT2_DecryptSplatBytes(dataList.GetRange(8, 8).ToArray(), v);
			data[0] = ctx.EndTables[0][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v0, ctx.EndMasks[0]) ^ ctx.EndXor[0])];
			data[1] = ctx.EndTables[1][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v0, ctx.EndMasks[1]) ^ ctx.EndXor[1])];
			data[2] = ctx.EndTables[2][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v0, ctx.EndMasks[2]) ^ ctx.EndXor[2])];
			data[3] = ctx.EndTables[3][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v0, ctx.EndMasks[3]) ^ ctx.EndXor[3])];
			data[4] = ctx.EndTables[4][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v0, ctx.EndMasks[4]) ^ ctx.EndXor[4])];
			data[5] = ctx.EndTables[5][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v0, ctx.EndMasks[5]) ^ ctx.EndXor[5])];
			data[6] = ctx.EndTables[6][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v0, ctx.EndMasks[6]) ^ ctx.EndXor[6])];
			data[7] = ctx.EndTables[7][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v0, ctx.EndMasks[7]) ^ ctx.EndXor[7])];
			data[8] = ctx.EndTables[8][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v, ctx.EndMasks[8]) ^ ctx.EndXor[8])];
			data[9] = ctx.EndTables[9][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v, ctx.EndMasks[9]) ^ ctx.EndXor[9])];
			data[10] = ctx.EndTables[10][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v, ctx.EndMasks[10]) ^ ctx.EndXor[10])];
			data[11] = ctx.EndTables[11][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v, ctx.EndMasks[11]) ^ ctx.EndXor[11])];
			data[12] = ctx.EndTables[12][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v, ctx.EndMasks[12]) ^ ctx.EndXor[12])];
			data[13] = ctx.EndTables[13][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v, ctx.EndMasks[13]) ^ ctx.EndXor[13])];
			data[14] = ctx.EndTables[14][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v, ctx.EndMasks[14]) ^ ctx.EndXor[14])];
			data[15] = ctx.EndTables[15][(int)(Tfit2CbcCipher.TFIT2_DecryptSquashBytes(v, ctx.EndMasks[15]) ^ ctx.EndXor[15])];
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x000398A0 File Offset: 0x00037AA0
		private static void TFIT2_DecryptBlock(Tfit2CbcCipher.Tfit2Context ctx, ulong[][] key, byte[] input, byte[] output)
		{
			byte[] temp = new byte[16];
			Array.Copy(input, temp, 16);
			Tfit2CbcCipher.TFIT2_DecryptRoundA(ctx, temp);
			Tfit2CbcCipher.TFIT2_DecryptRoundB(ctx, 0, temp, key[0]);
			Tfit2CbcCipher.TFIT2_DecryptRoundB(ctx, 1, temp, key[1]);
			for (int i = 2; i < 16; i++)
			{
				Tfit2CbcCipher.TFIT2_DecryptRoundC(ctx, i, temp, key[i]);
			}
			Tfit2CbcCipher.TFIT2_DecryptRoundB(ctx, 16, temp, key[16]);
			Tfit2CbcCipher.TFIT2_DecryptRoundD(ctx, temp);
			Buffer.BlockCopy(temp, 0, output, 0, 16);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00039920 File Offset: 0x00037B20
		public Tfit2CbcCipher(Tfit2CbcCipher.Tfit2Key key, byte[] iv, Tfit2CbcCipher.Tfit2Context ctx)
		{
			this.iv_ = new byte[iv.Length];
			Array.Copy(iv, this.iv_, 16);
			this.keys_ = key.Data.Skip(1).ToArray<ulong[]>();
			this.ctx_ = ctx;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00039970 File Offset: 0x00037B70
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
				Array.Copy(input, i, Next_iv, 0, 16);
				byte[] outbytes = new byte[16];
				Array.Copy(input, i, bytes, 0, 16);
				Tfit2CbcCipher.TFIT2_DecryptBlock(this.ctx_, this.keys_, bytes, outbytes);
				for (int j = 0; j < 16; j++)
				{
					//input[i + j] = (outbytes[j] ^ Cur_iv[j]);
                    input[j] = (byte)(outbytes[j] ^ Cur_iv[j]);
                }
				Cur_iv = Next_iv;
			}
			Array.Copy(Cur_iv, this.iv_, 16);
			return input;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00039A7C File Offset: 0x00037C7C
		public static byte[] TFIT2_DecryptBytes(Tfit2CbcCipher.Tfit2Key key, byte[] iv, Tfit2CbcCipher.Tfit2Context ctx, byte[] input, int? start = null, int? lenght = null)
		{
			Tfit2CbcCipher tfit2CbcCipher = new Tfit2CbcCipher(key, iv, ctx);
			return tfit2CbcCipher.Decode(input, start, lenght);
		}

		// Token: 0x0400007A RID: 122
		public ulong[][] keys_;

		// Token: 0x0400007B RID: 123
		public Tfit2CbcCipher.Tfit2Context ctx_;

		// Token: 0x0400007C RID: 124
		public byte[] iv_;

		// Token: 0x0400007D RID: 125
		private const int TFIT2_BLOCK_SIZE = 16;

		// Token: 0x0200003C RID: 60
		public struct Block
		{
			// Token: 0x17000020 RID: 32
			// (get) Token: 0x0600020F RID: 527 RVA: 0x0003AB7C File Offset: 0x00038D7C
			// (set) Token: 0x06000210 RID: 528 RVA: 0x0003AB84 File Offset: 0x00038D84
			public ulong[] Masks { get; set; }

			// Token: 0x17000021 RID: 33
			// (get) Token: 0x06000211 RID: 529 RVA: 0x0003AB8D File Offset: 0x00038D8D
			// (set) Token: 0x06000212 RID: 530 RVA: 0x0003AB95 File Offset: 0x00038D95
			public uint Xor { get; set; }
		}

		// Token: 0x0200003D RID: 61
		public struct Round
		{
			// Token: 0x17000022 RID: 34
			// (get) Token: 0x06000213 RID: 531 RVA: 0x0003AB9E File Offset: 0x00038D9E
			// (set) Token: 0x06000214 RID: 532 RVA: 0x0003ABA6 File Offset: 0x00038DA6
			public ulong[] Lookup { get; set; }

			// Token: 0x17000023 RID: 35
			// (get) Token: 0x06000215 RID: 533 RVA: 0x0003ABAF File Offset: 0x00038DAF
			// (set) Token: 0x06000216 RID: 534 RVA: 0x0003ABB7 File Offset: 0x00038DB7
			public Tfit2CbcCipher.Block[] Blocks { get; set; }
		}

		// Token: 0x0200003E RID: 62
		public struct Tfit2Context
		{
			// Token: 0x17000024 RID: 36
			// (get) Token: 0x06000217 RID: 535 RVA: 0x0003ABC0 File Offset: 0x00038DC0
			// (set) Token: 0x06000218 RID: 536 RVA: 0x0003ABC8 File Offset: 0x00038DC8
			public ulong[][] InitTables { get; set; }

			// Token: 0x17000025 RID: 37
			// (get) Token: 0x06000219 RID: 537 RVA: 0x0003ABD1 File Offset: 0x00038DD1
			// (set) Token: 0x0600021A RID: 538 RVA: 0x0003ABD9 File Offset: 0x00038DD9
			public Tfit2CbcCipher.Round[] Rounds { get; set; }

			// Token: 0x17000026 RID: 38
			// (get) Token: 0x0600021B RID: 539 RVA: 0x0003ABE2 File Offset: 0x00038DE2
			// (set) Token: 0x0600021C RID: 540 RVA: 0x0003ABEA File Offset: 0x00038DEA
			public ulong[][] EndMasks { get; set; }

			// Token: 0x17000027 RID: 39
			// (get) Token: 0x0600021D RID: 541 RVA: 0x0003ABF3 File Offset: 0x00038DF3
			// (set) Token: 0x0600021E RID: 542 RVA: 0x0003ABFB File Offset: 0x00038DFB
			public byte[][] EndTables { get; set; }

			// Token: 0x17000028 RID: 40
			// (get) Token: 0x0600021F RID: 543 RVA: 0x0003AC04 File Offset: 0x00038E04
			// (set) Token: 0x06000220 RID: 544 RVA: 0x0003AC0C File Offset: 0x00038E0C
			public byte[] EndXor { get; set; }
		}

		// Token: 0x0200003F RID: 63
		public struct Tfit2Key
		{
			// Token: 0x04000C06 RID: 3078
			public ulong[][] Data;
		}
	}
}
