using System;
using System.Windows;
using System.Windows.Controls;

namespace SoftwareThemeDesigner
{
	public class RctSpinBox : RctTextBox
	{
		static RctSpinBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctSpinBox), new FrameworkPropertyMetadata(typeof(RctSpinBox)));
		}
	}
}
