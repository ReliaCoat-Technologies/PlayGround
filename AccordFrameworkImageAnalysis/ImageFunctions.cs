using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordFrameworkImageAnalysis
{
	public static class ImageFunctions
	{
		public static Bitmap convertToGrayscale(this Bitmap inputImage)
		{
			if (inputImage.PixelFormat == PixelFormat.Format8bppIndexed) return inputImage;

			var converter = new Grayscale(0.2125, 0.7154, 0.0721);

			return converter.Apply(inputImage);
		}

		public static Bitmap correctFlatField(this Bitmap inputImage, int blurRadius = 75)
		{
			var sw = new Stopwatch();
			sw.Start();

			var flatFieldCorrection = new FlatFieldCorrection();

			var result = flatFieldCorrection.Apply(inputImage);

			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);

			return result;
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
	}
}
