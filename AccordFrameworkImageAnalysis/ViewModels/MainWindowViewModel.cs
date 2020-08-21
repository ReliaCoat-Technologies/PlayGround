using System;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Drawing;
using System.Threading.Tasks;
using AccordFrameworkImageAnalysis.Utilities;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Linq;

namespace AccordFrameworkImageAnalysis.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Fields
		private Bitmap _originalImage;
		private Bitmap _workingImage;
		private Bitmap _thresholdedImage;
		private ObservableCollection<ContiguousPoreInfo> _poreInfoList;
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
		public ObservableCollection<ContiguousPoreInfo> poreInfoList
		{
			get { return _poreInfoList; }
			set { _poreInfoList = value; RaisePropertyChanged(() => poreInfoList); }
		}
		#endregion

		#region Commands
		public ICommand processImageCommand { get; set; }

		#endregion

		#region Constructor
		public MainWindowViewModel()
		{
			poreInfoList = new ObservableCollection<ContiguousPoreInfo>();
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

			var poreList = thresholdedImage.getBlobs().ToList();

			foreach (var blob in poreList)
				poreInfoList.Add(blob);

			var blankBitmap = new Bitmap(workingImage.Width, workingImage.Height, PixelFormat.Format24bppRgb);

			using (var gfx = Graphics.FromImage(blankBitmap))
			{
				var pen = new Pen(Brushes.Black);
				gfx.DrawRectangle(pen, new Rectangle(0, 0, blankBitmap.Width, blankBitmap.Height));
			}

			foreach(var pore in poreList)
				pore.drawPore(blankBitmap);

			thresholdedImage = blankBitmap;
		}
		#endregion
	}
}