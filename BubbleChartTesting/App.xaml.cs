using System.Windows;
using ReliaCoat.Common.UI.Controls;
using ReliaCoat.Common.UI.Extensions.SciChartExtensions;

namespace BubbleChartTesting
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			SoftwareThemeManager.instance.initialize(this, true);
			SciChartThemeManager.instance.initialize(true);

			var window = new MainWindow();
			window.Show();
		}
	}
}
