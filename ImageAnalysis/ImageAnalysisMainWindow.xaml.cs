using System;
using DevExpress.Xpf.Core;
using ImageAnalysis.ViewModels;

namespace ImageAnalysis
{
	public partial class ImageAnalysisMainWindow : ThemedWindow
	{
		public ImageAnalysisMainWindow()
		{
			InitializeComponent();

			var viewModel = new MainWindowViewModel();
			DataContext = viewModel;
		}
	}
}
