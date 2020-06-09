using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using SoftwareThemeDesigner.RoutedEvents;
using SoftwareThemeDesignerTester.ViewModels;

namespace SoftwareThemeDesignerTester
{
	public partial class MainWindow : ThemedWindow
	{
		public MainWindow()
		{
			var viewModel = new MainWindowViewModel();
			DataContext = viewModel;

			InitializeComponent();

			passwordBox.KeyUp += PasswordBoxOnKeyUp;

			checkBox1.Checked += CheckBoxOnChecked;
			checkBox2.Checked += CheckBoxOnChecked;
		}

		private void CheckBoxOnChecked(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Checkbox Checked");
		}

		private void PasswordBoxOnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
				Console.WriteLine($"Typed Password: {passwordBox.password}");
		}

		private void OnButtonClick(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Button Clicked");
		}

		private void BaseEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
		{
			Console.WriteLine($"DevExpress Drop-Down Value Changed: {comboBoxEdit.EditValue}");
		}

		private void ListBoxEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
		{
			var value = (listBoxEdit.EditValue as Country)?.countryName;
			Console.WriteLine($"DevExpress ListBox Value Changed: {value}");
		}

		private void SpinEdit_OnEditValueChanged(object sender, EditValueChangedEventArgs e)
		{
			var value = Convert.ToDouble(spinEdit.EditValue);
			Console.WriteLine($"DevExpress SpinBox Value Changed: {value}");
		}

		private void ListBox_OnSelected(object sender, SelectionChangedEventArgs e)
		{
			var value = (listBox.SelectedItem as Country)?.countryName;
			Console.WriteLine($"New ListBox Value Changed: {value}");
		}

		private void onSpin(object sender, SpinRoutedEventArgs e)
		{
			Console.WriteLine($"Spin Event Handled: Old Value = {e.oldValue}, New Value = {e.newValue}");
		}

		private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			Console.WriteLine($"Text Box Value Updated: {textBox.Text}");
		}

		private void onComboBoxItemChanged(object sender, RoutedEventArgs e)
		{
			var value = (comboBox.value as Country)?.countryName;
			Console.WriteLine($"New ComboBox Value Changed: {value}");
		}
	}
}
