using AccordFrameworkImageAnalysis.ViewModels;
using System.Windows;

namespace AccordFrameworkImageAnalysis
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var viewModel = new MainWindowViewModel();
			DataContext = viewModel;
		}
	}
}
