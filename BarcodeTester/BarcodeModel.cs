using System.Drawing;
using MongoDB.Bson;
using ZXing;
using ZXing.PDF417;

namespace  BarcodeTester
{
	public static class BarcodeModel
	{
		public static Bitmap generateBarcode(BarcodeFormat barcodeFormat)
		{
			var writer = new BarcodeWriter {Format = barcodeFormat};

			var objectId = ObjectId.GenerateNewId().ToString();

			var barcode = writer.Encode(objectId);

			var renderer = writer.Renderer;

			var bitmap = renderer.Render(barcode, barcodeFormat, objectId);

			return bitmap;
		}
	}
}
