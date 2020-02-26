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

				// Enables ripple effect when selecting text box.
				_searchTextBox.PreviewMouseLeftButtonDown += (s, e) =>
				{
					_rippleDecorator.doAnimation(e);
				};
			}
		}

		private void SearchTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Tab) return;

			SelectedItem = _hostPanel.Children.OfType<ListBoxItem>()
				.FirstOrDefault(x => x.IsVisible)
				?.Content;

			Focus();

			e.Handled = true;
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				acceptValue();
			}
			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				return;
			}

			_searchTextBox.Focus();

			base.OnPreviewKeyDown(e);
		}

		private void acceptValue()
		{
			var filteredChildren = _hostPanel
				.Children
				.OfType<ComboBoxItem>()
				.Select((x, i) => new { index = i, cbItem = x })
				.Where(x => x.cbItem.IsVisible)
				.ToList();

			var firstHighlightedIndex = filteredChildren.FirstOrDefault(x => x.cbItem.IsHighlighted)?.index ?? -1;

			if (!string.IsNullOrWhiteSpace(_searchTextBox.Text) && filteredChildren.Count > 0)
				SelectedIndex = firstHighlightedIndex > 0 ? firstHighlightedIndex : filteredChildren.First().index;
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
