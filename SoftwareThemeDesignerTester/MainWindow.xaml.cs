using System;
using System.Threading.Tasks;
using DevExpress.Xpf.Core;
using SoftwareThemeDesignerTester.ViewModels;

namespace SoftwareThemeDesignerTester
{
	public partial class MainWindow : ThemedWindow
	{
		private readonly MainWindowViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();

			_viewModel = new MainWindowViewModel();
			DataContext = _viewModel;

			Loaded += async (s,e) => await onLoadedAsync();
		}

		private async Task onLoadedAsync()
		{
			await _viewModel.initializeAsync();
		}
	}
}
