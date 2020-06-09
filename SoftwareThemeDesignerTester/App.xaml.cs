﻿using System;
using System.Windows;
using DevExpress.Xpf.Core;

namespace SoftwareThemeDesignerTester
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var theme = new Theme("RctDarkTheme", "DevExpress.Xpf.Themes.RctDarkTheme.v19.1")
			{
				AssemblyName = "DevExpress.Xpf.Themes.RctDarkTheme.v19.1"
			};

			Theme.RegisterTheme(theme);
		}
	}
}
