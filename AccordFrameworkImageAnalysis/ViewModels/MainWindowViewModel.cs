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
using Point = System.Windows.Point;

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
		private ContiguousPoreInfo _selectedPore;
		private double _pulloutDensity;
		private double _globularPorosity;
		private double _interlamellarPorosity;
		private double _crackDensity;
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
		public ContiguousPoreInfo selectedPore
		{
			get { return _selectedPore; }
			set { _selectedPore = value; RaisePropertyChanged(() => selectedPore); }
		}
		public double pulloutDensity
		{
			get { return _pulloutDensity; }
			set { _pulloutDensity = value; RaisePropertyChanged(() => pulloutDensity); }
		}
		public double globularPorosity
		{
			get { return _globularPorosity; }
			set { _globularPorosity = value; RaisePropertyChanged(() => globularPorosity); }
		}
		public double interlamellarPorosity
		{
			get { return _interlamellarPorosity; }
			set { _interlamellarPorosity = value; RaisePropertyChanged(() => interlamellarPorosity); }
		}
		public double crackDensity
		{
			get { return _crackDensity; }
			set { _crackDensity = value; RaisePropertyChanged(() => crackDensity); }
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

		public async Task processImageAsync(string fileName = null)
		{
			poreInfoList.Clear();

			var fileManager = new ImageFileManager();

			originalImage = await fileManager.openFileAsync(fileName);

			if (originalImage == null) return;

			var dialogViewModel = new ImageCropDialogViewModel(originalImage);
			var view = new ImageCropDialogView();
			view.setViewModel(dialogViewModel);

			var result = new RctDialog.Builder()
				.setContent(view)
				.setTitle("Crop Image")
				.setMessageBoxButton(MessageBoxButton.OKCancel)
				.setDimensions(800, 800)
				.build();

			if (result.prompt() != MessageBoxResult.OK) return;

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

			var totalArea = _analyzedImage.Height * analyzedImage.Width;

			pulloutDensity = getPorosity(PoreType.Pullout);
			globularPorosity = getPorosity(PoreType.Globular);
			interlamellarPorosity = getPorosity(PoreType.Interlamellar);
			crackDensity = getPorosity(PoreType.Crack);
		}

		private double getPorosity(PoreType poreType)
		{
			var totalArea = _analyzedImage.Height * analyzedImage.Width;

			var poreArea = _poreInfoList
				.Where(x => x.poreType == poreType)
				.Sum(x => x.area);

			return poreArea / totalArea;
		}

		public void getPoreHitPoint(Point hitPoint)
		{
			selectedPore?.drawPore(analyzedImage);

			var imageX = Convert.ToInt32(hitPoint.X * analyzedImage.Width);
			var imageY = Convert.ToInt32(hitPoint.Y * analyzedImage.Height);

			var rectangle = new Rectangle(0, 0, analyzedImage.Height, analyzedImage.Width);

			selectedPore = _poreInfoList
				.OrderBy(x => Math.Abs(x.centerX - imageX))
				.ThenBy(x => Math.Abs(x.centerY - imageY))
				.FirstOrDefault(x => x.isHit(imageX, imageY, rectangle));

			selectedPore?.selectPore(analyzedImage);

			RaisePropertyChanged(() => analyzedImage);
		}
		#endregion
	}
}