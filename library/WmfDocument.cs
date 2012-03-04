using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Oxage.Wmf.Records;

namespace Oxage.Wmf
{
	//Human-friendly reader and writer
	public class WmfDocument
	{
		public WmfDocument()
		{
			this.Records = new List<IBinaryRecord>();
			this.Records.Add(new WmfFormat());
			this.Records.Add(new WmfHeader());
			this.Records.Add(new WmfSetMapModeRecord());
			this.Records.Add(new WmfSetWindowOrgRecord());
			this.Records.Add(new WmfSetWindowExtRecord()); //Add SetWindowExtRecord for compatibility
		}

		public List<IBinaryRecord> Records
		{
			get;
			set;
		}

		public WmfFormat Format
		{
			get
			{
				return this.Records.FirstOrDefault(x => x is WmfFormat) as WmfFormat;
			}
		}

		public WmfHeader Header
		{
			get
			{
				return this.Records.FirstOrDefault(x => x is WmfHeader) as WmfHeader;
			}
		}

		public int Width
		{
			get
			{
				return this.Format.Right - this.Format.Left;
			}
			set
			{
				this.Format.Right = (ushort)(this.Format.Left + value);

				var ext = this.Records.FirstOrDefault(x => x is WmfSetWindowExtRecord) as WmfSetWindowExtRecord;
				if (ext != null)
				{
					ext.X = (short)(this.Format.Right - this.Format.Left);
				}
			}
		}

		public int Height
		{
			get
			{
				return this.Format.Bottom - this.Format.Top;
			}
			set
			{
				this.Format.Bottom = (ushort)(this.Format.Top + value);

				var ext = this.Records.FirstOrDefault(x => x is WmfSetWindowExtRecord) as WmfSetWindowExtRecord;
				if (ext != null)
				{
					ext.Y = (short)(this.Format.Bottom - this.Format.Top);
				}
			}
		}

		public void Load(string path)
		{
			using (var stream = File.OpenRead(path))
			{
				Load(stream);
			}
		}

		public void Load(Stream stream)
		{
			this.Records = new List<IBinaryRecord>();
			using (var reader = new WmfReader(stream))
			{
				while (true)
				{
					try
					{
						var record = reader.Read();
						if (record != null)
						{
							this.Records.Add(record);
						}
					}
					catch (EndOfStreamException)
					{
						//End of stream
						break;
					}
				}
			}
		}

		public void Save(string path)
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
			{
				Save(stream);
			}
		}

		public void Save(Stream stream)
		{
			this.Format.Checksum = this.Format.CalculateChecksum();

			if (!this.Records.OfType<WmfEndOfFileRecord>().Any())
			{
				this.Records.Add(new WmfEndOfFileRecord());
			}

			using (var writer = new WmfWriter(stream))
			{
				foreach (var record in this.Records)
				{
					writer.Write(record);
				}

				writer.FixHeader();
				writer.FixPadding();
			}
		}

		public string Dump()
		{
			var builder = new StringBuilder();
			foreach (var record in this.Records)
			{
				builder.AppendLine(record.Dump());
				builder.AppendLine();
			}
			return builder.ToString();
		}

		public void AddSelectObject(int index)
		{
			this.Records.Add(new WmfSelectObjectRecord() { ObjectIndex = (ushort)index });
		}

		public void AddDeleteObject(int index)
		{
			this.Records.Add(new WmfDeleteObjectRecord() { ObjectIndex = (ushort)index });
		}

		public void AddPolyline(IEnumerable<Point> points)
		{
			var list = points.ToList();
			this.Records.Add(new WmfPolylineRecord()
			{
				NumberOfPoints = (short)list.Count,
				Points = list
			});
		}

		public void AddPolyPolygon(IEnumerable<IEnumerable<Point>> polygons)
		{
			var list = polygons.ToList();

			var record = new WmfPolyPolygonRecord();
			record.NumberOfPolygons = (short)list.Count;
			record.PointsPerPolygon = new List<int>();
			record.Points = new List<Point>();

			foreach (var polygon in list)
			{
				var points = polygon.ToList();
				record.PointsPerPolygon.Add(points.Count);

				foreach (var point in polygon)
				{
					record.Points.Add(point);
				}
			}

			this.Records.Add(record);
		}

		public void AddRectangle(int x, int y, int width, int height)
		{
			var record = new WmfRectangleRecord();
			record.SetRectangle(new Rectangle(x, y, width, height));
			this.Records.Add(record);
		}

		public void AddPolyFillMode(PolyFillMode mode)
		{
			this.Records.Add(new WmfSetPolyFillModeRecord() { Mode = mode });
		}

		public void AddCreateBrushIndirect(Color color, BrushStyle style = BrushStyle.BS_SOLID, HatchStyle hatch = HatchStyle.HS_HORIZONTAL)
		{
			this.Records.Add(new WmfCreateBrushIndirectRecord()
			{
				Color = color,
				Style = style,
				Hatch = hatch
			});
		}

		public void AddCreatePenIndirect(Color color, PenStyle style = PenStyle.PS_SOLID, int size = 1)
		{
			this.Records.Add(new WmfCreatePenIndirectRecord()
			{
				Color = color,
				Style = style,
				Width = new Point(size, size)
			});
		}
	}
}
