using System;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Drawing;
using System.Threading.Tasks;
using AccordFrameworkImageAnalysis.Utilities;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using AccordFrameworkImageAnalysis.Views;
using ReliaCoat.Common.UI.Extensions.CustomUserControls;

namespace AccordFrameworkImageAnalysis.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Fields
		private Bitmap _originalImage;
		private Bitmap _croppedImage;
		private Bitmap _workingImage;
		private Bitmap _thresholdedImage;
		private Bitmap _analyzedImage;
		private ObservableCollection<ContiguousPoreInfo> _poreInfoList;
		
		#endregion

		#region Properties
		public Bitmap originalImage
		{
			get { return _originalImage; }
			set { _originalImage = value; RaisePropertyChanged(() => originalImage); }
		}
		public Bitmap croppedImage
		{
			get { return _croppedImage; }
			set { _croppedImage = value; RaisePropertyChanged(() => croppedImage); }
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
		public Bitmap analyzedImage
		{
			get { return _analyzedImage; }
			set { _analyzedImage = value; RaisePropertyChanged(() => analyzedImage); }
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

			var dialogViewModel = new ImageCropDialogViewModel(originalImage);
			var view = new ImageCropDialogView();
			view.setViewModel(dialogViewModel);

			var result = RctDialog.createDialog(view, "Crop Image", MessageBoxButton.OKCancel, null, 800, 800);

			if (result != MessageBoxResult.OK) return;

			croppedImage = originalImage.cropAtRectangle(dialogViewModel.cropRect);

			workingImage = croppedImage
				.convertToGrayscale()
				.correctFlatField();

			thresholdedImage = workingImage
				.otsuThresholdImage()
				.invertimage();

			var poreList = thresholdedImage.getBlobs().ToList();

			foreach (var blob in poreList)
				poreInfoList.Add(blob);

			var poreImage = new Bitmap(workingImage.Width, workingImage.Height, PixelFormat.Format24bppRgb);

			using (var gfx = Graphics.FromImage(poreImage))
			{
				var pen = new Pen(Brushes.Black);
				gfx.DrawRectangle(pen, new Rectangle(0, 0, poreImage.Width, poreImage.Height));
			}

			foreach (var pore in poreList)
				pore.drawPore(poreImage);

			analyzedImage = poreImage;
		}
		#endregion
	}
}