using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SoftwareThemeDesigner.RoutedEvents;

namespace SoftwareThemeDesigner
{
	public class RctSpinBox : RctTextBox
	{
		#region Statics
		private const double translucentOpacity = 0.6;
		#endregion

		#region Fields
		private Button _spinUpButton;
		private Button _spinDownButton;
		private TextBlock _prefixTextBlock;
		private TextBlock _suffixTextBlock;
		#endregion

		#region Dependency Properties
		private static readonly DependencyPropertyKey _formattedTextPropertyKey = DependencyProperty.RegisterReadOnly(nameof(formattedText), typeof(string), typeof(RctSpinBox), new PropertyMetadata());

		public static readonly DependencyProperty stringFormatProperty = DependencyProperty.Register(nameof(stringFormat), typeof(string), typeof(RctSpinBox), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty incrementProperty = DependencyProperty.Register(nameof(increment), typeof(double), typeof(RctSpinBox), new PropertyMetadata(1d));
		public static readonly DependencyProperty majorIncrementProperty = DependencyProperty.Register(nameof(majorIncrement), typeof(double), typeof(RctSpinBox), new PropertyMetadata(10d));
		public static readonly DependencyProperty valueProperty = DependencyProperty.Register(nameof(value), typeof(double), typeof(RctSpinBox), new FrameworkPropertyMetadata(0d, valueChangedCallback));
		public static readonly DependencyProperty formattedTextProperty = _formattedTextPropertyKey.DependencyProperty;
		public static readonly DependencyProperty prefixProperty = DependencyProperty.Register(nameof(prefix), typeof(string), typeof(RctSpinBox), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty suffixProperty = DependencyProperty.Register(nameof(suffix), typeof(string), typeof(RctSpinBox), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty minValueProperty = DependencyProperty.Register(nameof(minValue), typeof(double), typeof(RctSpinBox), new PropertyMetadata(double.MinValue));
		public static readonly DependencyProperty maxValueProperty = DependencyProperty.Register(nameof(maxValue), typeof(double), typeof(RctSpinBox), new PropertyMetadata(double.MaxValue));
		#endregion

		#region Callbacks
		private static void valueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var spinBox = d as RctSpinBox;
			spinBox?.onValueChanged((double)e.NewValue);
		}
		#endregion

		#region Routed Events
		public static readonly RoutedEvent spinEvent = EventManager.RegisterRoutedEvent(nameof(spin), RoutingStrategy.Bubble, typeof(SpinRoutedEventHandler), typeof(RctSpinBox));
		#endregion

		#region Delegates
		public event SpinRoutedEventHandler spin
		{
			add { AddHandler(spinEvent, value); }
			remove { RemoveHandler(spinEvent, value); }
		}
		#endregion

		#region Properties
		public string stringFormat
		{
			get { return GetValue(stringFormatProperty).ToString(); }
			set { SetValue(stringFormatProperty, value); }
		}
		public double increment
		{
			get { return (double)GetValue(incrementProperty); }
			set { SetValue(incrementProperty, value); }
		}
		public double majorIncrement
		{
			get { return (double)GetValue(majorIncrementProperty); }
			set { SetValue(majorIncrementProperty, value); }
		}
		public double value
		{
			get { return (double)GetValue(valueProperty); }
			set { SetValue(valueProperty, value); }
		}
		public string formattedText => GetValue(formattedTextProperty).ToString();
		public string prefix
		{
			get { return GetValue(prefixProperty).ToString(); }
			set { SetValue(prefixProperty, value); }
		}
		public string suffix
		{
			get { return GetValue(suffixProperty).ToString(); }
			set { SetValue(suffixProperty, value); }
		}

		public double minValue
		{
			get { return (double)GetValue(minValueProperty); }
			set { SetValue(minValueProperty, value); }
		}
		public double maxValue
		{
			get { return (double)GetValue(maxValueProperty); }
			set { SetValue(maxValueProperty, value); }
		}
		#endregion

		#region Constructor
		static RctSpinBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctSpinBox), new FrameworkPropertyMetadata(typeof(RctSpinBox)));
		}
		#endregion

		#region Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Text = 0.ToString(stringFormat);
			SetValue(_formattedTextPropertyKey, $"{prefix}{Text}{suffix}");

			_prefixTextBlock = GetTemplateChild("PART_PrefixTextBlock") as TextBlock;
			if (_prefixTextBlock != null)
			{
				_prefixTextBlock.Text = prefix;
			}

			_suffixTextBlock = GetTemplateChild("PART_SuffixTextBlock") as TextBlock;
			if (_suffixTextBlock != null)
			{
				_suffixTextBlock.Text = suffix;
			}

			_spinUpButton = GetTemplateChild("PART_SpinUpButton") as Button;
			if (_spinUpButton != null)
			{
				_spinUpButton.Click += onSpinUpButtonClicked;
			}

			_spinDownButton = GetTemplateChild("PART_SpinDownButton") as Button;
			if (_spinDownButton != null)
			{
				_spinDownButton.Click += onSpinDownButtonClicked;
			}
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			double outValue;

			if (string.IsNullOrWhiteSpace(Text))
				Text = "0";

			if (double.TryParse(Text, out outValue))
			{
				value = outValue;
			}

			// Allows user to type in decimal point.
			if (Text.Last() == '.' && Text.Count(x => x == '.') == 1)
				return;

			Text = value.ToString(stringFormat);

			CaretIndex = Text.Length;
		}

		protected override void OnGotMouseCapture(MouseEventArgs e)
		{
			base.OnGotMouseCapture(e);
			onMouseCaptureChanged(e, true);
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);
			onMouseCaptureChanged(e, false);
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			onActivatedChanged(true);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			onActivatedChanged(false);
		}

		protected virtual void onValueChanged(double newValue)
		{
			if (value > maxValue)
				value = maxValue;

			if (value < minValue)
				value = minValue;

			Text = value.ToString(stringFormat);
			SetValue(_formattedTextPropertyKey, $"{prefix}{Text}{suffix}");
		}

		private void onMouseCaptureChanged(RoutedEventArgs e, bool isActivated)
		{
			// Prevents shading changes when clicking on spin buttons.
			if (e.OriginalSource == _spinUpButton || e.OriginalSource == _spinDownButton) return;
			onActivatedChanged(isActivated);
		}

		private void onActivatedChanged(bool isActivated)
		{
			if (isActivated)
			{
				SelectAll();
				_prefixTextBlock.Opacity = translucentOpacity;
				_suffixTextBlock.Opacity = translucentOpacity;
			}
			else
			{
				_prefixTextBlock.Opacity = 1;
				_suffixTextBlock.Opacity = 1;
			}
		}

		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			raiseSpinEvent(e.Delta <= 0);
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Up:
					raiseSpinEvent();
					return;
				case Key.Down:
					raiseSpinEvent(true);
					return;
				default:
					return;
			}
		}

		protected virtual void onSpinUpButtonClicked(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			raiseSpinEvent();
		}

		protected virtual void onSpinDownButtonClicked(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			raiseSpinEvent(true);
		}

		protected void raiseSpinEvent(bool decrease = false)
		{
			var oldValue = value;

			var shiftDown = Keyboard.Modifiers == ModifierKeys.Shift;

			if (decrease) value -= shiftDown ? majorIncrement : increment;
			else value += shiftDown ? majorIncrement : increment;

			RaiseEvent(new SpinRoutedEventArgs(spinEvent, oldValue, value));

			SelectAll();
		}
		#endregion
	}
}
