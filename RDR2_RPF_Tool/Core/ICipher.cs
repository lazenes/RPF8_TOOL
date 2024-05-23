using System;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x0200001B RID: 27
	public interface ICipher
	{
		// Token: 0x0600017E RID: 382
		byte[] Decode(byte[] input, int? start = null, int? lenght = null);
	}
}
