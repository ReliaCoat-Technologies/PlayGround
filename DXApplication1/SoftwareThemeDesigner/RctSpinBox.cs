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
		#region Fields
		private Button _spinUpButton;
		private Button _spinDownButton;
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty stringFormatProperty = DependencyProperty.Register(nameof(stringFormat), typeof(string), typeof(RctSpinBox), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty incrementProperty = DependencyProperty.Register(nameof(increment), typeof(double), typeof(RctSpinBox), new PropertyMetadata(1d));
		public static readonly DependencyProperty majorIncrementProperty = DependencyProperty.Register(nameof(majorIncrement), typeof(double), typeof(RctSpinBox), new PropertyMetadata(10d));
		public static readonly DependencyProperty valueProperty = DependencyProperty.Register(nameof(value), typeof(double), typeof(RctSpinBox), new FrameworkPropertyMetadata(0d, valueChangedCallback));

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

			Text = 0.ToString();

			_spinUpButton = GetTemplateChild("PATH_SpinUpButton") as Button;
			if (_spinUpButton != null)
			{
				_spinUpButton.Click += onSpinUpButtonClicked;
			}

			_spinDownButton = GetTemplateChild("PATH_SpinDownButton") as Button;
			if (_spinDownButton != null)
			{
				_spinDownButton.Click += onSpinDownButtonClicked;
			}
		}

		protected virtual void onValueChanged(double value)
		{
			Text = value.ToString(stringFormat);
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
			SelectAll();
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			SelectAll();
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
			raiseSpinEvent();
		}

		protected virtual void onSpinDownButtonClicked(object sender, RoutedEventArgs e)
		{
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
