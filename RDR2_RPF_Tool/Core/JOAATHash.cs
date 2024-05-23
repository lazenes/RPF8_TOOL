using System;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x0200001C RID: 28
	public static class JOAATHash
	{
		// Token: 0x0600017F RID: 383 RVA: 0x00006B98 File Offset: 0x00004D98
		public static uint Calc(string input)
		{
			uint hash = 0U;
			foreach (char item in input)
			{
				byte byte_of_data = (byte)item;
				hash += (uint)byte_of_data;
				hash += hash << 10;
				hash ^= hash >> 6;
			}
			hash += hash << 3;
			hash ^= hash >> 11;
			return hash + (hash << 15);
		}
	}
}
