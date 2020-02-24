using System;
using System.Globalization;
using System.Windows.Data;

namespace SoftwareThemeDesigner.Utilities
{
	public class ToUpperConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stringValue = value as string;
			return stringValue?.ToUpper() ?? value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}