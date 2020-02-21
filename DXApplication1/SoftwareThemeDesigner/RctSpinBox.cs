using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SoftwareThemeDesigner
{
	public class RctSpinBox : RctTextBox
	{
		#region Fields
		private Button spinUpButton;
		private Button spinDownButton;
		private double value;
		#endregion

		#region Routed Events
		public static readonly RoutedEvent SpinEvent = EventManager.RegisterRoutedEvent(nameof(Spin), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RctSpinBox));
		#endregion

		#region Delegates
		public event RoutedEventHandler Spin
		{
			add { AddHandler(SpinEvent, value); }
			remove { RemoveHandler(SpinEvent, value); }
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
			else
			{
				Text = value.ToString();
			}
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
			value += 1;
			Text = value.ToString();
		}

		protected virtual void onSpinDownButtonClicked(object sender, RoutedEventArgs e)
		{
			value -= 1;
			Text = value.ToString();
		}
		#endregion
	}
}
