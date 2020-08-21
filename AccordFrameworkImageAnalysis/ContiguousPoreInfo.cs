using Accord.Imaging;
using Accord.Imaging.Moments;
using System;
using System.Drawing;

namespace AccordFrameworkImageAnalysis
{
	public class ContiguousPoreInfo
	{
		#region Properties
		public Blob blob { get; }
		public Bitmap managedImage { get; }
		public double area { get; }
		public double momentArea { get; }
		public double centerX { get; }
		public double centerY { get; }
		public double angle { get; }
		public double height { get; }
		public double width { get; }
		public double aspectRatio { get; }
		public PoreType poreType { get; }
		#endregion

		#region Constructor
		public ContiguousPoreInfo(Blob blob)
		{
			this.blob = blob;

			managedImage = blob.Image.ToManagedImage();

			var moment = new CentralMoments(managedImage);

			area = blob.Area;
			momentArea = moment.Mu00 / 255;
			centerX = blob.CenterOfGravity.X;
			centerY = blob.CenterOfGravity.Y;
			angle = 180 - moment.GetOrientation() * 180 / Math.PI;
			height = moment.GetSize().Height;
			width = moment.GetSize().Width;
			aspectRatio = height / width;
			poreType = aspectRatio > 6 ? PoreType.Interlamellar : PoreType.Globular;
		}
		#endregion

		#region Methods
		public void drawPore(Bitmap poreBitmap)
		{
			var firstPoreRect = blob.Rectangle;
			var firstPoreImage = managedImage;

			var color = poreType == PoreType.Globular ? Color.OrangeRed : Color.LimeGreen;

			for (int i = 0; i < firstPoreRect.Width; i++)
			{
				for (int j = 0; j < firstPoreRect.Height; j++)
				{
					var pixel = firstPoreImage.GetPixel(i, j);

					if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0) continue;

					poreBitmap.SetPixel(i + firstPoreRect.X, j + firstPoreRect.Y, color);
				}
			}
		}
		#endregion
	}
}