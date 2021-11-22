using System;
using System.Collections.Generic;
using System.Windows;
using ReliaCoat.Common;
using ReliaCoat.Common.UI.ValueConverters;
using ZXing;

namespace BarcodeTester
{
	public partial class MainWindow : Window
	{
		private readonly IDictionary<string, BarcodeFormat> _comboBoxItems;

		public MainWindow()
		{
			InitializeComponent();

			_comboBoxItems = EnumOperations.getEnumDescriptions<BarcodeFormat>();

			var keys = _comboBoxItems.Keys;

			foreach (var key in keys)
			{
				formatComboBox.Items.Add(key);
			}
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			if (formatComboBox.SelectedItem == null)
			{
				return;
			}

			var format = _comboBoxItems[formatComboBox.SelectedItem.ToString()];

			try
			{
				var barcodeBitmap = BarcodeModel.generateBarcode(format);

				BarcodeImageHolder.Source = DrawingToMediaConverter.getBitmapImage(barcodeBitmap);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
