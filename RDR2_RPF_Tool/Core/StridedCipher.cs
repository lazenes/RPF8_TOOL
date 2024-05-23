using System;
using System.Collections.Generic;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x02000026 RID: 38
	public class StridedCipher
	{
		// Token: 0x060001A6 RID: 422 RVA: 0x00038A28 File Offset: 0x00036C28
		internal static void UnpackConfig(byte config, ref long head_length, ref long block_length, ref long block_stride)
		{
            byte head_config = (byte)(config & 3);

            bool flag = (config & 3) > 0;
			if (flag)
			{
				head_length = 1024L << (int)(head_config * 2);
			}
			byte length_config = (byte)(config >> 2 & 7);
			bool flag2 = (config >> 2 & 7) != 0;
			if (flag2)
			{
				byte stride_config = (byte)(config >> 5 & 7);
				block_length = 1024L << (int)length_config;
				block_stride = (long)(stride_config + 1) << 16;
			}
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00038A8C File Offset: 0x00036C8C
		public static List<List<long>> UnpackConfig(byte config, long file_size, long chunk_size)
		{
			List<List<long>> results = new List<List<long>>();
			long offset = 0L;
			Action<long, long> Update = delegate(long start, long end)
			{
				start = Math.Max(start, offset);
				end = Math.Min(end, file_size);
				bool flag4 = start < end;
				if (flag4)
				{
					results.Add(new List<long>
					{
						start,
						end
					});
					offset = end;
				}
			};
			long tail_length = 1024L;
			long tail_offset = file_size - tail_length;
			long head_length = 0L;
			long block_length = 0L;
			long block_stride = 0L;
			StridedCipher.UnpackConfig(config, ref head_length, ref block_length, ref block_stride);
			Update(0L, head_length);
			bool flag = head_length < tail_offset;
			if (flag)
			{
				bool flag2 = block_length != 0L || block_stride != 0L;
				if (flag2)
				{
					bool flag3 = block_stride == chunk_size && tail_offset < block_stride && block_stride < file_size;
					if (flag3)
					{
						offset = block_stride;
					}
					else
					{
						long block = (offset != 0L) ? block_stride : 0L;
						while (block + block_length <= tail_offset)
						{
							Update(block, block + block_length);
							block += block_stride;
						}
					}
				}
				Update(tail_offset, file_size);
			}
			return results;
		}
	}
}
