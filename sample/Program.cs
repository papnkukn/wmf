using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Oxage.Wmf;
using Oxage.Wmf.Records;

namespace Oxage
{
	public class Program
	{
		public static void Main(string[] args)
		{
#if DEBUG
			if (Debugger.IsAttached)
			{
				var wmf = new WmfDocument();
				wmf.Load("sample.wmf");
				//CreateFeatureSample("sample.wmf");
				return;
			}
#endif

			if (args.Length != 2)
			{
				Console.WriteLine("Usage:");
				Console.WriteLine("  wmf <command> <path>");
				Console.WriteLine();
				Console.WriteLine("Commands:");
				Console.WriteLine("  dump     Lists records in human-readable form");
				Console.WriteLine("  sample   Creates a sample WMF file");
				Console.WriteLine();
				Console.WriteLine("Examples:");
				Console.WriteLine("  wmf sample file.wmf");
				Console.WriteLine("  wmf dump file.wmf");
				return;
			}

			string action = args[0];
			string path = args[1];

			switch (action)
			{
				case "dump":
					var wmf = new WmfDocument();
					wmf.Load(path);
					string output = wmf.Dump();
					Console.WriteLine(output);
					break;

				case "sample":
					//CreateFeatureSample(path);
					CreateOfficialSample(path);
					break;

				default:
					Console.Write("Unknown command: " + action);
					break;
			}
		}

		/// <summary>
		/// Creates an example from the official specification document.
		/// </summary>
		/// <param name="path"></param>
		public static void CreateOfficialSample(string path)
		{
			var wmf = WmfHelper.GetExampleFromSpecificationDocument();
			wmf.Records.Insert(0, new WmfFormat() { Right = 0x0096, Bottom = 0x0046, Unit = 100 });
			wmf.Save(path);
		}

		/// <summary>
		/// Creates an example with implemented features.
		/// </summary>
		/// <param name="path"></param>
		public static void CreateFeatureSample(string path)
		{
			var wmf = new WmfDocument();
			wmf.Width = 1000;
			wmf.Height = 1000;
			wmf.Format.Unit = 288;
			wmf.AddPolyFillMode(PolyFillMode.WINDING);

			//Define fill brush
			wmf.AddCreateBrushIndirect(Color.Blue, BrushStyle.BS_SOLID);
			wmf.AddSelectObject(0);

			//Define stroke brush
			wmf.AddCreatePenIndirect(Color.Black, PenStyle.PS_SOLID, 1);
			wmf.AddSelectObject(1);

			//Shapes
			wmf.AddRectangle(new Point(100, 100), new Size(800, 800), 50);
			wmf.AddPolyPolygon(new List<IEnumerable<Point>>()
			{
			  //Polygon 1
			  new List<Point>()
			  {
			    new Point(150, 150),
					new Point(150 + 700, 150),
					new Point(150 + 700, 150 + 700),
					new Point(150, 150 + 700),
					new Point(150, 150),
			  },
			  //Polygon 2
				//new List<Point>()
				//{
				//  new Point()
				//}
			});
			wmf.AddEllipse(new Point(500, 500), new Point(250, 200));
			wmf.AddCircle(new Point(500, 500), 100);

			//Text
			wmf.AddCreateFontIndirect("Arial", -48);
			wmf.AddSelectObject(2);
			wmf.AddTextColor(Color.Green);
			wmf.AddTextAlignment(TextAlignmentMode.TA_CENTER);
			wmf.AddText("Hello World!", new Point(500, 24));

			wmf.AddDeleteObject(0);
			wmf.AddDeleteObject(1);
			wmf.AddDeleteObject(2);
			wmf.Save(path);
		}
	}
}
