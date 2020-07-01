using System.IO;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using NUnit.Framework;

using SkiaSharp;

namespace NUnitTestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        private static byte[] ToBytes(SKPaint paint, string text) => SkiaSharp.StringUtilities.GetEncodedText(text, paint.TextEncoding);

        [Test]
        public void CanDrawCustom()
        {
	        var stream = new MemoryStream();
	        var _rect = SKRect.Create(500, 500);
	        using (var wstream = new SKManagedWStream(stream))
	        using (var writer = new SKXmlStreamWriter(wstream))
	        using (var svg = SKSvgCanvas.Create(_rect, writer))
	        {
		        unsafe
		        {
			        var paint = new SKPaint
			        {
				        Color = SKColors.Red,
				        Style = SKPaintStyle.Fill
			        };
			        var array = new SKAttr[3] {
				        new SKAttr(),
				        new SKAttr(),
				        new SKAttr()
			        };
			        SKCustomElement customElement = default;

			        fixed (void* ta = ToBytes(paint, "SampleElement"))
			        {
				        fixed (SKAttr* aaPtr = array)
				        {
					        customElement = new SKCustomElement() { Attrs = aaPtr, Length = 3, Name = ta };
				        }
			        }
			        svg.DrawRect(SKRect.Create(10, 10, 80, 80), paint);
			        svg.DrawCustom(_rect, paint, &customElement);
		        }
	        }

	        stream.Position = 0;

	        using (var reader = new StreamReader(stream))
	        {
		        var xml = reader.ReadToEnd();
		        var xdoc = XDocument.Parse(xml);

		        var svg = xdoc.Root;
		        var ns = svg.Name.Namespace;

		        Assert.Equals(ns + "svg", svg.Name);
		        Assert.Equals("100", svg.Attribute("width")?.Value);
		        Assert.Equals("100", svg.Attribute("height")?.Value);

		        var rect = svg.Element(ns + "rect");
		        Assert.Equals(ns + "rect", rect.Name);
		        Assert.Equals("rgb(255,0,0)", rect.Attribute("fill")?.Value);
		        Assert.Equals("none", rect.Attribute("stroke")?.Value);
		        Assert.Equals("10", rect.Attribute("x")?.Value);
		        Assert.Equals("10", rect.Attribute("y")?.Value);
		        Assert.Equals("80", rect.Attribute("width")?.Value);
		        Assert.Equals("80", rect.Attribute("height")?.Value);
	        }
        }
	}
}
