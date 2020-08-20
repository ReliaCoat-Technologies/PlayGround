using System;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm;
using ImageAnalysis.Utilities;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Imaging.Moments;
using Accord.Math.Geometry;

namespace ImageAnalysis.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Fields
		private Bitmap _originalImage;
		private Bitmap _workingImage;
		private Bitmap _thresholdedImage;
		private Bitmap _firstBlobImage;
		#endregion

		#region Properties
		public Bitmap originalImage
		{
			get { return _originalImage; }
			set { _originalImage = value; RaisePropertyChanged(() => originalImage); }
		}
		public Bitmap workingImage
		{
			get { return _workingImage; }
			set { _workingImage = value; RaisePropertyChanged(() => workingImage); }
		}
		public Bitmap thresholdedImage
		{
			get { return _thresholdedImage; }
			set { _thresholdedImage = value; RaisePropertyChanged(() => thresholdedImage); }
		}
		public Bitmap firstBlobImage
		{
			get { return _firstBlobImage; }
			set { _firstBlobImage = value; RaisePropertyChanged(() => firstBlobImage); }
		}
		#endregion

		#region Commands
		public ICommand processImageCommand { get; set; }
		#endregion

		#region Constructor
		public MainWindowViewModel()
		{
			processImageCommand = new DelegateCommand(async () => await processImageAsync());
		}

		private async Task openImageAsync()
		{
			var fileManager = new ImageFileManager();
			originalImage = await fileManager.openFileAsync();
		}

		private async Task processImageAsync()
		{
			await openImageAsync();

			if (originalImage == null) return;

			workingImage = originalImage
				.convertToGrayscale()
				.correctFlatField();

			thresholdedImage = workingImage
				.otsuThresholdImage()
				.invertimage();

			var blobCounter = new BlobCounter(thresholdedImage);

			var blobs = blobCounter
				.GetObjectsInformation()
				.OrderByDescending(x => x.Area)
				.ToList();

			var firstBlob = blobs.FirstOrDefault();

			blobCounter.ExtractBlobsImage(thresholdedImage, firstBlob, false);

			firstBlobImage = firstBlob.Image.ToManagedImage();

			var image = new Bitmap(firstBlobImage.Width, firstBlobImage.Height);

			var moment = new CentralMoments(firstBlobImage, 2);

			var area = firstBlob.Area;
			var momentArea = moment.Mu00 / 255;
			var cog = firstBlob.CenterOfGravity;
			var angle = moment.GetOrientation() * 180 / Math.PI;
			var size = moment.GetSize();
		}
		#endregion
	}
}