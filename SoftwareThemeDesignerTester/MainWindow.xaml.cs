using System;
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
		}
	}
}
