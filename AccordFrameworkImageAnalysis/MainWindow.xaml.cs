using System.Linq;
using AccordFrameworkImageAnalysis.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccordFrameworkImageAnalysis
{
	public partial class MainWindow : Window
	{
		private MainWindowViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();

			_viewModel = new MainWindowViewModel();
			DataContext = _viewModel;
		}

		private void onImageClick(object sender, MouseButtonEventArgs e)
		{
			var image = sender as Image;

			var position = e.GetPosition(image);

			var relativeX = position.X / image.ActualWidth;
			var relativeY = position.Y / image.ActualHeight;

			_viewModel.getPoreHitPoint(new Point(relativeX, relativeY));
		}

		private async void onDrop(object sender, DragEventArgs e)
		{
			var files = e.Data.GetData(DataFormats.FileDrop) as string[];

			if (files == null || !files.Any()) return;

			await _viewModel.processImageAsync(files.First());
		}
	}
}
