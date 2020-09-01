using Accord.Imaging;
using Accord.Imaging.Filters;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace AccordFrameworkImageAnalysis
{
	public static class ImageFunctions
	{
		public static Bitmap cropAtRectangle(this Bitmap inputImage, Rectangle rectangle)
		{
			var cropper = new Crop(rectangle);
			return cropper.Apply(inputImage);
		}

		public static Bitmap convertToGrayscale(this Bitmap inputImage)
		{
			if (inputImage.PixelFormat == PixelFormat.Format8bppIndexed) return inputImage;

			var converter = new Grayscale(0.2125, 0.7154, 0.0721);

			return converter.Apply(inputImage);
		}

		public static Bitmap correctFlatField(this Bitmap inputImage, int blurRadius = 75)
		{
			var flatFieldCorrection = new FlatFieldCorrection();

			return flatFieldCorrection.Apply(inputImage);
		}

		public static Bitmap invertimage(this Bitmap inputImage)
		{
			var inverter = new Invert();

			return inverter.Apply(inputImage);
		}

		public static Bitmap otsuThresholdImage(this Bitmap inputImage)
		{
			var thresholding = new OtsuThreshold();

			var rect = new Rectangle(0, 0, inputImage.Width, inputImage.Height);

			var thresholdValue = thresholding.CalculateThreshold(inputImage, rect);

			return thresholding.Apply(inputImage);
		}

		public static IEnumerable<ContiguousPoreInfo> getBlobs(this Bitmap inputImage)
		{
			var blobCounter = new BlobCounter(inputImage);

			var blobs = blobCounter
				.GetObjectsInformation()
				.OrderByDescending(x => x.Area)
				.ToList();

			foreach(var blob in blobs)
			{
				var edgePoints = blobCounter.GetBlobsEdgePoints(blob);
				blobCounter.ExtractBlobsImage(inputImage, blob, false);
				yield return new ContiguousPoreInfo(blob, edgePoints);
			}
		}
	}
}
