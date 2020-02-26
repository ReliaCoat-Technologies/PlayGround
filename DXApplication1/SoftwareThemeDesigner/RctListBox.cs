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
		private TextBox _searchTextBox;
		private StackPanel _hostPanel;
		private RctRippleDecorator _rippleDecorator;
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
			_hostPanel = GetTemplateChild("PART_HostPanel") as StackPanel;

			_rippleDecorator = GetTemplateChild("PART_RippleDecorator") as RctRippleDecorator;

			_searchTextBox = GetTemplateChild("PART_SearchTextBox") as TextBox;
			if (_searchTextBox != null)
			{
				_searchTextBox.PreviewKeyDown += SearchTextBoxOnPreviewKeyDown;
				_searchTextBox.TextChanged += onSearchBoxTextChanged;
			}
		}

		private void SearchTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				SelectedItem = _hostPanel.Children.OfType<ListBoxItem>()
					.FirstOrDefault(x => x.IsVisible)
					?.Content;

				Focus();

				e.Handled = true;
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				base.OnPreviewKeyDown(e);
				return;
			}
			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				return;
			}

			_searchTextBox.Focus();

			base.OnPreviewKeyDown(e);
		}

		private void onSearchBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			foreach (var item in _hostPanel.Children.OfType<ListBoxItem>())
			{
				var isAvailable = item.Content.containsFilterInOrder(_searchTextBox.Text, DisplayMemberPath);
				item.Visibility = isAvailable ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		#endregion
	}
}
