using System;
using System.Windows;
using System.Windows.Controls;

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
				_passwordBox.PasswordChanged += (s, e) => password = _passwordBox.Password;
			}
		}

		public void clear()
		{
			_passwordBox.Clear();
		}
		#endregion
	}
}
