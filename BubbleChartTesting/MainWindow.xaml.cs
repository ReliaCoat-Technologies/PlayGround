using System.Windows;
using ReliaCoat.Common.UI.Extensions.SciChartExtensions;

namespace BubbleChartTesting
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			SciChartThemeManager.instance.setTheme(sciChartSurface);

			DataContext = new MainWindowViewModel();
		}
	}
}
