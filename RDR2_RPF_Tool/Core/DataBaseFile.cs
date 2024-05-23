using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Helper;

namespace RDR2_RPF_Tool.Core
{
	// Token: 0x0200001A RID: 26
	public class DataBaseFile
	{
		// Token: 0x06000174 RID: 372 RVA: 0x000064ED File Offset: 0x000046ED
		public DataBaseFile(byte[] bytes)
		{
			this.stream = new MStream(bytes);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00006504 File Offset: 0x00004704
		public static string[] ExportTexts(byte[] data)
		{
			return new DataBaseFile(data).ExportText();
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00006524 File Offset: 0x00004724
		public static byte[] ImportTexts(byte[] data, string[] text)
		{
			return new DataBaseFile(data).ImportText(text);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00006544 File Offset: 0x00004744
		public static byte[] ImportTexts(byte[] data, Dictionary<uint, string> text)
		{
			return new DataBaseFile(data).ImportText(text);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00006564 File Offset: 0x00004764
		public string[] ExportText()
		{
			List<string> strings = new List<string>();
			foreach (KeyValuePair<uint, DataBaseFile.TextEntry> entry in this.GetTextEntries())
			{
				strings.Add("0x" + entry.Value.hash.ToString("x2") + "=" + this.stream.GetStringValue((int)entry.Value.StringLength, false, (int)entry.Value.Offset, Encoding.UTF8).Trim(new char[1]));
			}
			return strings.ToArray();
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000663C File Offset: 0x0000483C
		public byte[] ImportText(string[] TextTable)
		{
			TextTable = (from x in TextTable
			where x.Contains("=")
			select x).ToArray<string>();
			Dictionary<uint, DataBaseFile.TextEntry> Entries = this.GetTextEntries();
			foreach (string str in TextTable)
			{
				string[] Line = str.Split(new char[]
				{
					'='
				}, 2);
				Line[0] = Line[0].Trim();
				bool flag = Line[0].StartsWith("0x");
				uint hash;
				if (flag)
				{
					hash = Convert.ToUInt32(Line[0], 16);
				}
				else
				{
					hash = JOAATHash.Calc(Line[0].ToLower());
				}
				string text = Line[1];
				bool flag2 = !Entries.ContainsKey(hash);
				if (!flag2)
				{
					DataBaseFile.TextEntry Entry = Entries[hash];
					this.stream.Seek(Entry.TextBlockOffset, SeekOrigin.Begin);
					this.stream.SetInt64Value(this.OffsetPut(this.stream.Length), true, -1, Endian.Little);
					this.stream.SetInt64Value((long)(Encoding.UTF8.GetBytes(text).Length + 1), true, -1, Endian.Little);
					this.stream.Seek(this.stream.Length, SeekOrigin.Begin);
					this.stream.SetStringValueN(text, true, -1, Encoding.UTF8);
					this.stream.SetPadding(0);
				}
			}
			byte[] bytes = this.stream.ToArray();
			this.Header.SetOrignalSize(bytes.Length);
			RSC8.CreateRsc8(ref bytes, this.Header);
			return bytes;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000067DC File Offset: 0x000049DC
		public byte[] ImportText(Dictionary<uint, string> TextTable)
		{
			Dictionary<uint, DataBaseFile.TextEntry> Entries = this.GetTextEntries();
			foreach (KeyValuePair<uint, string> str in TextTable)
			{
				bool flag = !Entries.ContainsKey(str.Key);
				if (!flag)
				{
					DataBaseFile.TextEntry Entry = Entries[str.Key];
					this.stream.Seek(Entry.TextBlockOffset, SeekOrigin.Begin);
					this.stream.SetInt64Value(this.OffsetPut(this.stream.Length), true, -1, Endian.Little);
					this.stream.SetInt64Value((long)(Encoding.UTF8.GetBytes(str.Value).Length + 1), true, -1, Endian.Little);
					this.stream.Seek(this.stream.Length, SeekOrigin.Begin);
					this.stream.SetStringValueN(str.Value, true, -1, Encoding.UTF8);
					this.stream.SetPadding(0);
				}
			}
			byte[] bytes = this.stream.ToArray();
			this.Header.SetOrignalSize(bytes.Length);
			RSC8.CreateRsc8(ref bytes, this.Header);
			return bytes;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00006928 File Offset: 0x00004B28
		public Dictionary<uint, DataBaseFile.TextEntry> GetTextEntries()
		{
			Dictionary<uint, DataBaseFile.TextEntry> textEntries = new Dictionary<uint, DataBaseFile.TextEntry>();
			this.Header = RSC8.ReadRsc8(this.stream);
			bool flag = this.Header.Magic != RSC8.RSC8Magic;
			if (flag)
			{
				throw new Exception("Invalid RSC8 File!");
			}
			bool flag2 = this.Header.GetResourceId() != GetResourceType.Text;
			if (flag2)
			{
				throw new Exception("this not text file resource!");
			}
			this.stream.DeleteBytes(16, true, -1);
			this.stream.SetPosition(16L);
			int Containeroffset = this.FixedOffset(this.stream.GetInt64Value(true, -1, Endian.Little));
			int offsetsCount = this.stream.GetIntValue(true, -1, Endian.Little);
			this.stream.Skip(4L);
			this.stream.Skip(4L);
			this.stream.Seek((long)Containeroffset, SeekOrigin.Begin);
			IEnumerable<long> offsets = from x in this.stream.GetArray<long>(offsetsCount, true, -1, Endian.Little)
			where x != 0L
			select x;
			foreach (long offset in offsets)
			{
				int Cur_offset = this.FixedOffset(offset);
				while (Cur_offset != 0)
				{
					DataBaseFile.TextEntry textentry = default(DataBaseFile.TextEntry);
					this.stream.Seek((long)Cur_offset, SeekOrigin.Begin);
					textentry.hash = (uint)this.stream.GetInt64Value(true, -1, Endian.Little);
					textentry.TextBlockOffset = (long)(this.FixedOffset(this.stream.GetInt64Value(true, -1, Endian.Little)) + 16);
					Cur_offset = this.FixedOffset(this.stream.GetInt64Value(true, -1, Endian.Little));
					this.stream.Seek(textentry.TextBlockOffset, SeekOrigin.Begin);
					textentry.Offset = (long)this.FixedOffset(this.stream.GetInt64Value(true, -1, Endian.Little));
					textentry.StringLength = this.stream.GetInt64Value(true, -1, Endian.Little);
					textEntries.Add(textentry.hash, textentry);
				}
			}
			return textEntries;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00006B60 File Offset: 0x00004D60
		public int FixedOffset(long value)
		{
			return (int)(value & 16777215L);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00006B7C File Offset: 0x00004D7C
		public long OffsetPut(long value)
		{
			return value | 1342177280L;
		}

		// Token: 0x04000054 RID: 84
		private IStream stream;

		// Token: 0x04000055 RID: 85
		private RSC8.RSC8Info Header;

		// Token: 0x02000034 RID: 52
		public struct TextEntry
		{
			// Token: 0x17000013 RID: 19
			// (get) Token: 0x060001CF RID: 463 RVA: 0x0003A529 File Offset: 0x00038729
			// (set) Token: 0x060001D0 RID: 464 RVA: 0x0003A531 File Offset: 0x00038731
			public uint hash { get; set; }

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x060001D1 RID: 465 RVA: 0x0003A53A File Offset: 0x0003873A
			// (set) Token: 0x060001D2 RID: 466 RVA: 0x0003A542 File Offset: 0x00038742
			public long TextBlockOffset { get; set; }

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x060001D3 RID: 467 RVA: 0x0003A54B File Offset: 0x0003874B
			// (set) Token: 0x060001D4 RID: 468 RVA: 0x0003A553 File Offset: 0x00038753
			public long Offset { get; set; }

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x060001D5 RID: 469 RVA: 0x0003A55C File Offset: 0x0003875C
			// (set) Token: 0x060001D6 RID: 470 RVA: 0x0003A564 File Offset: 0x00038764
			public long StringLength { get; set; }
		}
	}
}
