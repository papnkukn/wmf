using System.Drawing;
using System.IO;
using System.Text;
using System;

namespace Oxage.Wmf.Records
{
	[WmfRecord(Type = RecordType.META_ELLIPSE, Size = 7)]
    public class WmfEllipseRecord : WmfRectangleRecord
	{
        public WmfEllipseRecord()
            : base()
		{
		}

        public void SetEllipse(Point center, Point radius)
        {
            this.SetRectangle(new Rectangle(
                new Point(center.X - radius.X, center.Y - radius.Y),
                new Size(radius.X + radius.X, radius.Y + radius.Y)
                ));
        }
	}
}
