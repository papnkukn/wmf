using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Oxage.Wmf.Records
{
	[WmfRecord(Type = RecordType.META_SELECTPALETTE, Size = 4)]
	public class WmfSelectPalette : WmfBinaryRecord
	{
		public WmfSelectPalette()
		{
		}

		public short Palette
		{
			get;
			set;
		}

		public override void Read(BinaryReader reader)
		{
			this.Palette = reader.ReadInt16();
		}

		public override void Write(BinaryWriter writer)
		{
			base.Write(writer);
			writer.Write(this.Palette);
		}

		protected override void Dump(StringBuilder builder)
		{
			base.Dump(builder);
			builder.AppendLine("Palette: " + this.Palette);
		}
	}
}
