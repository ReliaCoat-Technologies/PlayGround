using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SoftwareThemeDesigner
{
	[TemplatePart(Name = "PART_Label", Type = typeof(TextBlock))]
	public class RctTextBox : TextBox
	{
		#region Fields
		private BindingExpression _bindingExpression;
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty labelTextProperty = DependencyProperty.Register(nameof(labelText), typeof(string), typeof(RctTextBox));
		public static readonly DependencyProperty labelFontSizeProperty = DependencyProperty.Register(nameof(labelFontSize), typeof(double), typeof(RctTextBox),
			new FrameworkPropertyMetadata(14d));
		public static readonly DependencyProperty labelTextColorProperty = DependencyProperty.Register(nameof(labelTextColor), typeof(Brush), typeof(RctTextBox));
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
		#endregion

		#region Constructor
		static RctTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctTextBox), new FrameworkPropertyMetadata(typeof(RctTextBox)));
			FontSizeProperty.OverrideMetadata(typeof(RctTextBox), new FrameworkPropertyMetadata(24d));
			BorderBrushProperty.OverrideMetadata(typeof(RctTextBox), new FrameworkPropertyMetadata(Brushes.White));
		}
		#endregion

		#region Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_bindingExpression = GetBindingExpression(TextProperty);
			if (_bindingExpression?.ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
				throw new InvalidEnumArgumentException("TextEditMod Warning: Setting UpdateSourceTrigger to PropertyChanged will cause source to change while typing. Suggested UpdateSourceTrigger for this control is LostFocus.");

			var label = GetTemplateChild("PART_Label") as TextBlock;
			if (label != null)
			{
				if (labelTextColor == null)
					labelTextColor = Foreground;

				label.Text = labelText;
				label.FontSize = labelFontSize;
				label.Foreground = labelTextColor;
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				_bindingExpression?.UpdateSource();
			}

			base.OnPreviewKeyDown(e);
		}

		protected override void OnGotMouseCapture(MouseEventArgs e)
		{
			base.OnGotMouseCapture(e);
			SelectAll();
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			SelectAll();
		}
		#endregion
	}
}
