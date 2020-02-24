using System;
using System.Windows;
using System.Windows.Controls;

namespace SoftwareThemeDesigner
{
	public class RctButton : Button
	{
		static RctButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctButton), new FrameworkPropertyMetadata(typeof(RctButton)));
		}
	}
}
