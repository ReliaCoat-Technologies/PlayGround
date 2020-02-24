using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using SoftwareThemeDesigner.RoutedEvents;
using SoftwareThemeDesigner.ViewModels;

namespace SoftwareThemeDesigner
{
	public partial class MainWindow : ThemedWindow
	{
		public MainWindow()
		{
			var viewModel = new MainWindowViewModel();
			DataContext = viewModel;

			InitializeComponent();

			spinBox.spin += onSpin;

			passwordBox.KeyUp += PasswordBoxOnKeyUp;

			checkBox1.Checked += CheckBoxOnChecked;
			checkBox2.Checked += CheckBoxOnChecked;
		}

		private void CheckBoxOnChecked(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Checkbox Checked");
		}

		private void onSpin(object sender, SpinRoutedEventArgs e)
		{
			Console.WriteLine($"Spin Event Handled: Old Value = {e.oldValue}, New Value = {e.newValue}");
		}

		private void PasswordBoxOnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				Console.WriteLine($"Typed Password: {passwordBox.password}");
				passwordBox.clear();
			}
		}

		private void OnButtonClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Button Clicked");
		}
	}
}
