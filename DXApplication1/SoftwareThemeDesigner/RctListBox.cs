using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SoftwareThemeDesigner.Utilities;

namespace SoftwareThemeDesigner
{
	public class RctListBox : ListBox
	{
		#region Fields
		private TextBox searchTextBox;
		private StackPanel hostPanel;
		#endregion

		#region Constructor
		static RctListBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctListBox), new FrameworkPropertyMetadata(typeof(RctListBox)));
		}
		#endregion

		#region Methods
		public override void OnApplyTemplate()
		{
			hostPanel = GetTemplateChild("PART_HostPanel") as StackPanel;

			searchTextBox = GetTemplateChild("PART_SearchTextBox") as TextBox;
			if (searchTextBox != null)
			{
				searchTextBox.TextChanged += onSearchBoxTextChanged;
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				return;
			}
			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				return;
			}

			searchTextBox.Focus();

			base.OnPreviewKeyDown(e);
		}

		private void onSearchBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			foreach (var item in hostPanel.Children.OfType<ListBoxItem>())
			{
				var isAvailable = item.Content.containsFilterInOrder(searchTextBox.Text, DisplayMemberPath);
				item.Visibility = isAvailable ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		#endregion
	}
}
