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
			labelTextProperty.AddOwner(typeof(PasswordBox));
			labelFontSizeProperty.AddOwner(typeof(PasswordBox));
			labelTextColorProperty.AddOwner(typeof(PasswordBox));
		}
		#endregion

		#region Methods

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_passwordBox = (PasswordBox)GetTemplateChild("PART_PasswordBox");
			if (_passwordBox != null)
			{
				// Forwards text from internal password box to current password box.
				_passwordBox.PasswordChanged += (s, e) => password = _passwordBox.Password;
				_passwordBox.GotFocus += (s, e) => updateVisualState(true);
				_passwordBox.GotMouseCapture += (s, e) => updateVisualState(true);
				_passwordBox.LostFocus += (s, e) => updateVisualState(true);
				_passwordBox.LostMouseCapture += (s, e) => updateVisualState(true);
			}
		}

		private void updateVisualState(bool useTransitions)
		{
			Console.WriteLine($"Password Box In Focus: {_passwordBox.IsFocused}");

			VisualStateManager.GoToState(this, _passwordBox.IsFocused
					? "PasswordFocused"
					: "Normal",
				useTransitions);
		}

		public void clear()
		{
			_passwordBox.Clear();
		}
		#endregion
	}
}
