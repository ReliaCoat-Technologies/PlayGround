using System.Drawing;
using MongoDB.Bson;
using ZXing;

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

			var bitmap = renderer.Render(barcode, BarcodeFormat.CODE_39, objectId);

			return bitmap;
		}
	}
}
