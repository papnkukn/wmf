using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System;

namespace Oxage.Wmf.Records
{
	[WmfRecord(Type = RecordType.META_MOVETO, Size = 5)]
    public class WmfMoveToRecord : WmfLineToRecord
	{
        public WmfMoveToRecord()
            : base()
		{

		}
	}
}
