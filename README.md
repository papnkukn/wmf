# Windows Metafile Library

Note: this project moved from https://wmf.codeplex.com/

## Project Description
The library supports reading and writing WMF files. Source code is written in pure .NET from scratch following the Windows Metafile Format Specification.

## Technical Information
* Name: **Window Metafile Format (WMF)**
* File extension: **.wmf, .emf**
* Mime type: **image/x-wmf, image/x-emf**
* Classification: vector/raster image
* Identifier: **D7 CD C6 9A** (first four bytes)
* Documentation: [WMF Specification](http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/WindowsMetafileFormat(wmf)Specification.pdf)

## Why WMF?
WMF is all about records - recorded sequence of actions. As opposite to other vector or raster image formats that define final image appearance, WMF contains intermediate states. Those states can be modified at specific working stage or can be "played" back to render an image. From other point of view WMF file natively contains "undo" history.

Another advantage of WMF is to support both vector and raster elements in one file. Disadvantage are lack of features (e.g. no element grouping like layers or trees), 16-bit limitations, bad extensibility due to old image format.

WMF can be replaced by EPS but since Windows has good integration with the WMF it is still useful in some cases: clipboard - transfering images with copy/paste, Clip Art images from MS Office are WMF, etc.

## Notice
This library does not fully implement Windows Metafile Format Specification. Current library version (v0.3) supports **only basic WMF records** but the source code is well structured so the missing records can easily be implemented if needed. I am not planning to develop other WMF records as the current version fulfills my requirements for another project.

## Examples

Create a simple WMF image with rectangle

```csharp
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
```

Read a WMF file and display human-readable values for analysis

```csharp
WmfDocument wmf = new WmfDocument();
wmf.Load(path);

string dump = wmf.Dump();
Console.WriteLine(dump);
```

Read records from WMF file

```csharp
WmfDocument wmf = new WmfDocument();
wmf.Load(path);

foreach (var record in wmf.Records)
{
  if (record is WmfCreateBrushIndirectRecord)
  {
    var brush = record as WmfCreateBrushIndirectRecord;
    Console.WriteLine("Color: " + brush.Color);
    Console.WriteLine("Style: " + brush.Style);
    Console.WriteLine("Hatch: " + brush.Hatch);
  }
  else if (record is WmfRectangleRecord)
  {
    var rectangle = record as WmfRectangleRecord;
    //Do something with rectangle...
  }
}
```
