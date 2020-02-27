using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SoftwareThemeDesigner
{
	public class RctPasswordBox : RctTextBox
	{
		#region Fields
		private PasswordBox _passwordBox;
		private bool _isLosingFocus;
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty passwordProperty = DependencyProperty.Register(nameof(password), typeof(string), typeof(RctPasswordBox), new PropertyMetadata(""));
		#endregion

		#region Properties
		public string password
		{
			get { return (string)GetValue(passwordProperty); }
			set { SetValue(passwordProperty, value); }
		}
		#endregion

		#region Statics
		static RctPasswordBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctPasswordBox), new FrameworkPropertyMetadata(typeof(RctPasswordBox)));
		}
		#endregion

		#region Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_passwordBox = (PasswordBox)GetTemplateChild("PART_PasswordBox");
			if (_passwordBox != null)
			{
				_passwordBox.PasswordChanged += (s, e) => password = _passwordBox.Password;
				_passwordBox.LostFocus += PasswordBoxOnLostKeyboardFocus;
			}
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			_passwordBox.Focus();

			Focusable = false; // Prevents focusing on control when focus is gained when de-focusing password box.
		}

		private void PasswordBoxOnLostKeyboardFocus(object sender, RoutedEventArgs routedEventArgs)
		{
			var request = new TraversalRequest(Keyboard.Modifiers == ModifierKeys.Shift
				? FocusNavigationDirection.Previous
				: FocusNavigationDirection.Next);

			MoveFocus(request);

			Focusable = true; // Re-enables focus on control
		}

		public void clear()
		{
			_passwordBox.Clear();
		}
		#endregion
	}
}
