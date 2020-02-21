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
		private Button spinUpButton;
		private Button spinDownButton;
		private double value;
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty stringFormatProperty = DependencyProperty.Register(nameof(stringFormat), typeof(string), typeof(RctSpinBox), new PropertyMetadata(string.Empty));
		#endregion

		#region Routed Events
		public static readonly RoutedEvent spinEvent = EventManager.RegisterRoutedEvent(nameof(spin),
			RoutingStrategy.Bubble,
			typeof(SpinRoutedEventHandler),
			typeof(RctSpinBox));
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

			spinUpButton = GetTemplateChild("PATH_SpinUpButton") as Button;
			if (spinUpButton != null)
			{
				spinUpButton.Click += onSpinUpButtonClicked;
			}

			spinDownButton = GetTemplateChild("PATH_SpinDownButton") as Button;
			if (spinDownButton != null)
			{
				spinDownButton.Click += onSpinDownButtonClicked;
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
			SelectAll();
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			SelectAll();
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

			if (decrease) value -= 1;
			else value += 1;

			Text = value.ToString(stringFormat);

			RaiseEvent(new SpinRoutedEventArgs(spinEvent, oldValue, value));
		}
		#endregion
	}
}
