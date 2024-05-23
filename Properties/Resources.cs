using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ModInstaller.Properties
{
	// Token: 0x0200000D RID: 13
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x0600003E RID: 62 RVA: 0x00002F9D File Offset: 0x0000119D
		internal Resources()
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002FA8 File Offset: 0x000011A8
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				bool flag = Resources.resourceMan == null;
				if (flag)
				{
					ResourceManager temp = new ResourceManager("ModInstaller.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = temp;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002FF0 File Offset: 0x000011F0
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00003007 File Offset: 0x00001207
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00003010 File Offset: 0x00001210
		internal static byte[] ModFile
		{
			get
			{
				object obj = Resources.ResourceManager.GetObject("ModFile", Resources.resourceCulture);
				return (byte[])obj;
			}
		}

		// Token: 0x04000038 RID: 56
		private static ResourceManager resourceMan;

		// Token: 0x04000039 RID: 57
		private static CultureInfo resourceCulture;
	}
}
