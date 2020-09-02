using Accord.Imaging;
using Accord.Imaging.Moments;
using System;
using System.Drawing;
using Accord.Math.Geometry;
using Accord;
using System.Collections.Generic;
using System.Linq;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace AccordFrameworkImageAnalysis
{
	public class ContiguousPoreInfo
	{
		#region Properties
		public Blob blob { get; set; }
		public List<IntPoint> hullPoints { get; set; }
		public Bitmap managedImage { get; set; }
		public Bitmap hullImage { get; set; }
		public double area { get; set; }
		public double momentArea { get; set; }
		public double centerX { get; set; }
		public double centerY { get; set; }
		public double angle { get; set; }
		public double height { get; set; }
		public double width { get; set; }
		public double aspectRatio { get; set; }
		public double convexity { get; set; }
		public PoreType poreType { get; set; }
		#endregion

		#region Constructor
		public ContiguousPoreInfo(Blob blobInput, BlobCounter blobCounter, Bitmap inputImage)
		{
			blob = blobInput;

			blobCounter.ExtractBlobsImage(inputImage, blob, false);

			managedImage = blob.Image.ToManagedImage();

			getConvexity();

			var moment = new CentralMoments(managedImage);

			area = blob.Area;
			momentArea = moment.Mu00 / 255;
			centerX = blob.CenterOfGravity.X;
			centerY = blob.CenterOfGravity.Y;
			angle = 180 - moment.GetOrientation() * 180 / Math.PI;
			height = moment.GetSize().Height;
			width = moment.GetSize().Width;
			aspectRatio = height / width;

			if (convexity > 0.9 && aspectRatio < 1.5)
				poreType = PoreType.Globular;
			else if (aspectRatio > 6)
			{
				if (angle < 45 || angle > 135)
					poreType = PoreType.Interlamellar;
				else
					poreType = PoreType.Crack;
			}
			else
				poreType = PoreType.Pullout;
		}

		private void getConvexity()
		{
			var hull = new GrahamConvexHull();
			hullPoints = hull.FindHull(blob.Image.CollectActivePixels());

			var xMin = hullPoints.Min(x => x.X);
			var yMin = hullPoints.Min(x => x.Y);

			var hullDrawPoints = hullPoints
				.Select(x => { return new System.Drawing.Point(x.X - xMin, x.Y - yMin); })
				.ToArray();

			hullImage = new Bitmap(managedImage.Width, managedImage.Height, PixelFormat.Format24bppRgb);

			using (var graphics = Graphics.FromImage(hullImage))
			{
				var bgPen = new Pen(Brushes.Black);
				graphics.DrawRectangle(bgPen, 0, 0, hullImage.Width, hullImage.Height);

				var foregroundPen = new Pen(Brushes.White);
				graphics.DrawPolygon(foregroundPen, hullDrawPoints);
				graphics.FillPolygon(Brushes.White, hullDrawPoints);
			}

			var statistics = new ImageStatistics(hullImage);
			var hullArea = statistics.PixelsCountWithoutBlack;

			convexity = (double)blob.Area / hullArea;
		}
		#endregion

		#region Methods
		public void drawPore(Bitmap poreBitmap)
		{
			var color = Color.Black;

			switch (poreType)
			{
				case PoreType.Pullout:
					color = Color.Red;
					break;
				case PoreType.Globular:
					color = Color.Goldenrod;
					break;
				case PoreType.Interlamellar:
					color = Color.LimeGreen;
					break;
				case PoreType.Crack:
					color = Color.White;
					break;
			}

			colorPore(poreBitmap, blob.Rectangle, managedImage, color);
		}

		public void selectPore(Bitmap poreBitmap)
		{
			colorPore(poreBitmap, blob.Rectangle, managedImage, Color.DodgerBlue);
		}

		public bool isHit(int X, int Y, Rectangle rect)
		{
			var blobRectangle = blob.Rectangle;
			var activePixels = blob.Image.CollectActivePixels();

			if (!activePixels.Any()) return false;

			var activeMinX = activePixels.Min(x => x.X);
			var activeMinY = activePixels.Min(x => x.Y);
			var activeMaxX = activePixels.Max(x => x.X);
			var activeMaxY = activePixels.Max(x => x.Y);

			return activePixels
				.Where(x => x.X + blobRectangle.X == X)
				.Any(x => x.Y + blobRectangle.Y == Y);
		}

		private static void colorPore(Bitmap poreBitmap, Rectangle firstPoreRect, Bitmap firstPoreImage, Color color)
		{
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