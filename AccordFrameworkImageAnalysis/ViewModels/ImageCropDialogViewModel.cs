using System;
using System.Drawing;
using DevExpress.Mvvm;

namespace AccordFrameworkImageAnalysis.ViewModels
{
	public class ImageCropDialogViewModel : ViewModelBase
	{
		#region Fields
		private Bitmap _bitmap;
		private int _imageX0;
		private int _imageX1;
		private int _imageY0;
		private int _imageY1;
		#endregion

		#region Properties
		public Bitmap bitmap
		{
			get { return _bitmap; }
			set { _bitmap = value; RaisePropertyChanged(() => bitmap); }
		}
		public int imageX0
		{
			get { return _imageX0; }
			set { _imageX0 = value; RaisePropertyChanged(() => imageX0); }
		}
		public int imageX1
		{
			get { return _imageX1; }
			set { _imageX1 = value; RaisePropertyChanged(() => imageX1); }
		}
		public int imageY0
		{
			get { return _imageY0; }
			set { _imageY0 = value; RaisePropertyChanged(() => imageY0); }
		}
		public int imageY1
		{
			get { return _imageY1; }
			set { _imageY1 = value; RaisePropertyChanged(() => imageY1); }
		}
		public Rectangle cropRect { get; private set; }
		#endregion

		#region Constructor
		public ImageCropDialogViewModel(Bitmap inputBitmap)
		{
			bitmap = inputBitmap;

			var imageSize = bitmap.Size;

			imageX1 = imageSize.Width;
			imageY1 = imageSize.Height;

			cropRect = new Rectangle(imageX0, imageY0, imageX1 - imageX0, imageY1 - imageY0);
		}
		#endregion

		#region Methods
		public void setCropDimensions(double scaleX0, double scaleY0, double scaleX1, double scaleY1)
		{
			var imageSize = bitmap.Size;

			imageX0 = Convert.ToInt32(imageSize.Width * scaleX0);
			imageY0 = Convert.ToInt32(imageSize.Height * scaleY0);
			imageX1 = Convert.ToInt32(imageSize.Width * scaleX1);
			imageY1 = Convert.ToInt32(imageSize.Height * scaleY1);

			cropRect = new Rectangle(imageX0, imageY0, imageX1 - imageX0, imageY1 - imageY0);
		}
		#endregion
	}
}