using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SoftwareThemeDesigner
{
	public class RctCheckBox : CheckBox
	{
		#region Constants
		private const string uncheckedPathData = "M19,3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3M19,5V19H5V5H19Z";
		private const string checkedPathData = "M10,17L5,12L6.41,10.58L10,14.17L17.59,6.58L19,8M19,3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3Z";
		private const string indeterminatePathData = "M19,3H5A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5A2,2 0 0,0 19,3M19,19H5V5H19V19M17,17H7V7H17V17Z";
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty labelTextProperty = DependencyProperty.Register(nameof(labelText), typeof(string), typeof(RctCheckBox));
		public static readonly DependencyProperty labelFontSizeProperty = DependencyProperty.Register(nameof(labelFontSize), typeof(double), typeof(RctCheckBox),
			new FrameworkPropertyMetadata(16d));
		public static readonly DependencyProperty labelTextColorProperty = DependencyProperty.Register(nameof(labelTextColor), typeof(Brush), typeof(RctCheckBox));
		public static readonly DependencyProperty sizeProperty = DependencyProperty.Register(nameof(size), typeof(double), typeof(RctCheckBox),
			new FrameworkPropertyMetadata(20d));
		#endregion

		#region Fields
		private Path _checkBoxShape;
		private Border _hostBorder;
		private TextBlock _label;
		#endregion

		#region Properties
		public string labelText
		{
			get { return (string)GetValue(labelTextProperty); }
			set { SetValue(labelTextProperty, value); }
		}
		public double labelFontSize
		{
			get { return (double)GetValue(labelFontSizeProperty); }
			set { SetValue(labelFontSizeProperty, value); }
		}
		public Brush labelTextColor
		{
			get { return (Brush)GetValue(labelTextColorProperty); }
			set { SetValue(labelTextColorProperty, value); }
		}
		public double size
		{
			get { return (double)GetValue(sizeProperty); }
			set { SetValue(sizeProperty, value); }
		}
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
				_checkBoxShape.Height = size;
				_checkBoxShape.Width = size;

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

			_hostBorder = GetTemplateChild("PART_CheckBoxShape") as Border;

			if (GetValue(labelTextColorProperty) as Brush == null)
				SetValue(labelTextColorProperty, Foreground);

			_label = GetTemplateChild("PART_Label") as TextBlock;
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			if (e.Key != Key.Enter && e.Key != Key.Return) return;

			switch (IsChecked)
			{
				case false:
					IsChecked = true;
					return;
				case true:
					IsChecked = IsThreeState ? (bool?)null : false;
					return;
				case null:
					IsChecked = false;
					return;
			}
		}

		protected override void OnChecked(RoutedEventArgs e)
		{
			base.OnChecked(e);

			if (_checkBoxShape == null) return;
			_checkBoxShape.Data = Geometry.Parse(checkedPathData);
		}

		protected override void OnUnchecked(RoutedEventArgs e)
		{
			base.OnUnchecked(e);

			if (_checkBoxShape == null) return;
			_checkBoxShape.Data = Geometry.Parse(uncheckedPathData);
		}

		protected override void OnIndeterminate(RoutedEventArgs e)
		{
			base.OnIndeterminate(e);

			if (_checkBoxShape == null) return;
			_checkBoxShape.Data = Geometry.Parse(indeterminatePathData);
		}
		#endregion
	}
}
