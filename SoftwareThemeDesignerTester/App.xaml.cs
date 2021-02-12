using System;
using System.Windows;
using ReliaCoat.Common.UI.Controls;

namespace SoftwareThemeDesignerTester
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			SoftwareThemeManager.instance.initialize(this, true);

			var window = new ThemeTesterWindow();
			window.Show();
		}
	}
}
