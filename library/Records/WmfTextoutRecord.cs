using System.Drawing;
using System.IO;
using System.Text;

namespace Oxage.Wmf.Records
{
	[WmfRecord(Type = RecordType.META_TEXTOUT)] //Variable size
	public class WmfTextoutRecord : WmfBinaryRecord
	{
		public WmfTextoutRecord() : base()
		{
		}

		public short StringLength
		{
			get;
			set;
		}

		public string StringValue
		{
			get;
			set;
		}

		public short YStart
		{
			get;
			set;
		}

		public short XStart
		{
			get;
			set;
		}

		protected Encoding AnsiEncoding
		{
			get
			{
				return WmfHelper.GetAnsiEncoding();
			}
		}

		public override void Read(BinaryReader reader)
		{
			this.StringLength = reader.ReadInt16();

			if (this.StringLength > 0)
			{
				byte[] ansi = reader.ReadBytes(this.StringLength);
				this.StringValue = AnsiEncoding.GetString(ansi);
			}

			this.YStart = reader.ReadInt16();
			this.XStart = reader.ReadInt16();
		}

		public override void Write(BinaryWriter writer)
		{
			byte[] ansi = AnsiEncoding.GetBytes(this.StringValue ?? "");

			base.RecordSizeBytes = (uint)(6 /* RecordSize and RecordFunction */ + 2 + ansi.Length + 2 + 2 /* Parameters */);
			base.Write(writer);

			if (this.StringLength == 0)
			{
				this.StringLength = (short)ansi.Length;
			}

			writer.Write(this.StringLength);
			writer.Write(ansi);
			writer.Write(this.YStart);
			writer.Write(this.XStart);
		}

		protected override void Dump(StringBuilder builder)
		{
			base.Dump(builder);
			builder.AppendLine("StringLength: " + this.StringLength);
			builder.AppendLine("String: " + this.StringValue);
			builder.AppendLine("YStart: " + this.YStart);
			builder.AppendLine("XStart: " + this.XStart);
		}
	}
}
