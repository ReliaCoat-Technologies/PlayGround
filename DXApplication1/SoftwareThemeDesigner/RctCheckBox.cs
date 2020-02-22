using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SoftwareThemeDesigner
{
	public class RctCheckBox : CheckBox
	{
		#region Constants
		private const string uncheckedPathData = "M19,3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3M19,5V19H5V5H19Z";
		private const string checkedPathData = "M19,3H5A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5A2,2 0 0,0 19,3M19,5V19H5V5H19M10,17L6,13L7.41,11.58L10,14.17L16.59,7.58L18,9";
		private const string indeterminatePathData = "M19,3H5A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5A2,2 0 0,0 19,3M19,19H5V5H19V19M17,17H7V7H17V17Z";
		#endregion

		#region Fields
		private Path _checkBoxShape;
		#endregion

		#region Constructor
		static RctCheckBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctCheckBox), new FrameworkPropertyMetadata(typeof(RctCheckBox)));
		}
		#endregion

		#region Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_checkBoxShape = GetTemplateChild("PART_CheckBoxShape") as Path;

			if (_checkBoxShape != null)
			{
				switch (IsChecked)
				{
					case true:
						_checkBoxShape.Data = Geometry.Parse(checkedPathData);
						break;
					case false:
						_checkBoxShape.Data = Geometry.Parse(uncheckedPathData);
						break;
					case null:
						_checkBoxShape.Data = Geometry.Parse(indeterminatePathData);
						break;
				}
			}
		}

		protected override void OnChecked(RoutedEventArgs e)
		{
			if (_checkBoxShape == null) return;
			_checkBoxShape.Data = Geometry.Parse(checkedPathData);
		}

		protected override void OnUnchecked(RoutedEventArgs e)
		{
			if (_checkBoxShape == null) return;
			_checkBoxShape.Data = Geometry.Parse(uncheckedPathData);
		}

		protected override void OnIndeterminate(RoutedEventArgs e)
		{
			if (_checkBoxShape == null) return;
			_checkBoxShape.Data = Geometry.Parse(indeterminatePathData);
		}
		#endregion
	}
}
