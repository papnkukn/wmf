using System;
using System.Diagnostics;
using System.Drawing;
using Oxage.Wmf;

namespace Oxage
{
	public class Program
	{
		public static void Main(string[] args)
		{
#if DEBUG
			if (Debugger.IsAttached)
			{
				//var wmf = new WmfDocument();
				//wmf.Load("sample.wmf");
				CreateSample("sample.wmf");
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
					CreateSample(path);
					break;

				default:
					Console.Write("Unknown command: " + action);
					break;
			}
		}

		public static void CreateSample(string path)
		{
			var wmf = new WmfDocument();
			wmf.Width = 1000;
			wmf.Height = 1000;
			wmf.Format.Unit = 288;
			wmf.AddPolyFillMode(PolyFillMode.WINDING);
			wmf.AddCreateBrushIndirect(Color.Blue, BrushStyle.BS_SOLID);
			wmf.AddSelectObject(0);
			wmf.AddCreatePenIndirect(Color.Black, PenStyle.PS_SOLID, 1);
			wmf.AddSelectObject(1);
			wmf.AddRectangle(100, 100, 800, 800);
			wmf.AddDeleteObject(0);
			wmf.AddDeleteObject(1);
			wmf.Save(path);
		}
	}
}
