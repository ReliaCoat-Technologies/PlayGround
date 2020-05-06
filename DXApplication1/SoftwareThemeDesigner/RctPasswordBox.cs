using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SoftwareThemeDesigner
{
	[TemplatePart(Name = "PART_PasswordBox", Type = typeof(PasswordBox))]
	public class RctPasswordBox : RctTextBox
	{
		#region Fields
		private PasswordBox _passwordBox;
		private TextBox _textBox;
		private Button _viewPasswordButton;
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty passwordProperty = DependencyProperty.Register(nameof(password), typeof(string), typeof(RctPasswordBox), new PropertyMetadata(""));
		public static readonly DependencyProperty showViewPasswordButtonProperty = DependencyProperty.Register(nameof(showViewPasswordButton), typeof(bool), typeof(RctPasswordBox), new FrameworkPropertyMetadata(false));
		#endregion

		#region Properties
		public string password
		{
			get { return (string)GetValue(passwordProperty); }
			set { SetValue(passwordProperty, value); }
		}

		public bool showViewPasswordButton
		{
			get { return (bool) GetValue(showViewPasswordButtonProperty); }
			set { SetValue(showViewPasswordButtonProperty, value); }
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

			_textBox = (TextBox) GetTemplateChild("PART_TextBox");
			if (_textBox != null)
			{
				_textBox.Visibility = Visibility.Hidden;
			}

			_viewPasswordButton = (Button) GetTemplateChild("PART_ViewPasswordButton");
			if (_viewPasswordButton != null)
			{
				_viewPasswordButton.PreviewMouseDown += showPassword;
				_viewPasswordButton.PreviewMouseUp += hidePassword;
			}
		}

		private void hidePassword(object sender, MouseButtonEventArgs e)
		{
			_textBox.Text = null;
			_textBox.Visibility = Visibility.Hidden;
			_passwordBox.Visibility = Visibility.Visible;
			_passwordBox.Focus();
		}

		private void showPassword(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			_textBox.Text = password;
			_textBox.Visibility = Visibility.Visible;
			_passwordBox.Visibility = Visibility.Hidden;
		}

		protected override void OnGotMouseCapture(MouseEventArgs e)
		{
			_passwordBox.Focus();
			Focusable = false; // Prevents focusing on control when focus is gained when de-focusing password box.
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			_passwordBox.Focus();
			Focusable = false; // Prevents focusing on control when focus is gained when de-focusing password box.

			_passwordBox.SelectAll();
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
