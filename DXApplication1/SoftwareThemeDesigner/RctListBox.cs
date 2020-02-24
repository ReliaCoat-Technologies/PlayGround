using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SoftwareThemeDesigner
{
	public class RctListBox : ListBox
	{
		static RctListBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctListBox), new FrameworkPropertyMetadata(typeof(RctListBox)));
		}
	}
}
