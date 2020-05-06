using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using SoftwareThemeDesigner.Utilities;

namespace SoftwareThemeDesigner
{
	[TemplatePart(Name = "PART_Label", Type = typeof(TextBlock))]
	[TemplatePart(Name = "PART_SearchTextBox", Type = typeof(TextBox))]
	public class RctComboBox : ComboBox
	{
		#region Fields
		private readonly IList<ComboBoxItem> _allComboBoxItems;
		private readonly IDictionary<int, ComboBoxItem> _visibleComboBoxItems;
		private Popup _popup;
		private TextBox _searchTextBox;
		private TextBox _editableTextBox;
		private ItemsPresenter _hostPanel;
		private ToggleButton _toggleButton;
		private RctRippleDecorator _rippleDecorator;
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty labelTextProperty = DependencyProperty.Register(nameof(labelText), typeof(string), typeof(RctComboBox));
		public static readonly DependencyProperty labelFontSizeProperty = DependencyProperty.Register(nameof(labelFontSize), typeof(double), typeof(RctComboBox),
			new FrameworkPropertyMetadata(14d));
		public static readonly DependencyProperty labelTextColorProperty = DependencyProperty.Register(nameof(labelTextColor), typeof(Brush), typeof(RctComboBox));
		public static readonly DependencyProperty allowDeleteProperty = DependencyProperty.Register(nameof(allowDelete), typeof(bool), typeof(RctComboBox),
			new FrameworkPropertyMetadata(true));
		#endregion

		#region Properties
		public string labelText
		{
			get { return (string)GetValue(labelTextProperty); }
			set { SetValue(labelTextProperty, value); }
		}
		public double labelFontSize
		{
			get { return (double)GetValue(labelFontSizeProperty); }
			set { SetValue(labelFontSizeProperty, value); }
		}
		public Brush labelTextColor
		{
			get { return (Brush)GetValue(labelTextColorProperty); }
			set { SetValue(labelTextColorProperty, value); }
		}
		public bool allowDelete
		{
			get { return (bool)GetValue(allowDeleteProperty); }
			set { SetValue(allowDeleteProperty, value); }
		}
		protected int currentIndex => _visibleComboBoxItems.FirstOrDefault(x => x.Value.DataContext == SelectedItem).Key;
		#endregion

		#region Constructor
		static RctComboBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctComboBox), new FrameworkPropertyMetadata(typeof(RctComboBox)));
			FontSizeProperty.OverrideMetadata(typeof(RctComboBox), new FrameworkPropertyMetadata(24d));
		}

		public RctComboBox()
		{
			_allComboBoxItems = new List<ComboBoxItem>();
			_visibleComboBoxItems = new Dictionary<int, ComboBoxItem>();
		}
		#endregion

		#region Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_hostPanel = GetTemplateChild("PART_HostPanel") as ItemsPresenter;

			_editableTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
			if (_editableTextBox != null)
			{
				_editableTextBox.Focusable = false;
			}

			_searchTextBox = GetTemplateChild("PART_SearchTextBox") as TextBox;
			if (_searchTextBox != null)
			{
				_searchTextBox.TextChanged += onSearchBoxTextChanged;
			}

			_popup = GetTemplateChild("PART_Popup") as Popup;
			if (_popup != null)
			{
				_popup.Opened += onPopupOpened;
			}

			if (GetValue(labelTextColorProperty) as Brush == null)
				SetValue(labelTextColorProperty, Foreground);

			_label = GetTemplateChild("PART_Label") as TextBlock;

			_rippleDecorator = GetTemplateChild("PART_RippleDecorator") as RctRippleDecorator;

			_toggleButton = GetTemplateChild("PART_ToggleButton") as ToggleButton;
			if (_toggleButton != null)
			{
				_toggleButton.PreviewMouseLeftButtonDown += (s, e) => _rippleDecorator?.doAnimation(e);
			}

			ScrollViewer.SetCanContentScroll(this, false);

			var defaultGroupStyle = FindResource("defaultGroupStyle") as GroupStyle;
			if (defaultGroupStyle != null)
				GroupStyle.Add(defaultGroupStyle);
		}

		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			_allComboBoxItems?.Clear();
			_visibleComboBoxItems?.Clear();
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				IsDropDownOpen = false;
				return;
			}
			if (e.Key == Key.Delete || e.Key == Key.Back)
			{
				if (allowDelete && !IsDropDownOpen)
				{
					SelectedItem = null;
					return;
				}
			}
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				if (IsDropDownOpen)
					acceptValue();
			}
			if (e.Key == Key.Tab)
			{
				return;
			}
			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				if (!IsDropDownOpen)
				{
					IsDropDownOpen = true;
				}
				else if (_searchTextBox.IsFocused)
				{
					var firstItem = _visibleComboBoxItems.FirstOrDefault().Value;

					firstItem?.Focus();
					SelectedItem = firstItem?.DataContext;
				}
				else
				{
					if (e.Key == Key.Up)
						goToPreviousSelection();
					else
						goToNextSelection();
				}

				return;
			}

			_searchTextBox.Focus();

			base.OnPreviewKeyDown(e);
		}

		private void goToPreviousSelection()
		{
			if (SelectedItem == null)
			{
				SelectedIndex = _visibleComboBoxItems.LastOrDefault().Key;
				return;
			}

			var previous = _visibleComboBoxItems.LastOrDefault(x => x.Key < currentIndex);
			SelectedIndex = previous.Value == null ? _visibleComboBoxItems.LastOrDefault().Key : previous.Key;
		}

		private void goToNextSelection()
		{
			if (SelectedItem == null)
			{
				SelectedIndex = _visibleComboBoxItems.FirstOrDefault().Key;
				return;
			}

			var next = _visibleComboBoxItems.FirstOrDefault(x => x.Key > currentIndex);
			SelectedIndex = next.Value == null ? _visibleComboBoxItems.FirstOrDefault().Key : next.Key;
		}

		private void onSearchBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			_visibleComboBoxItems.Clear();

			for (var i = 0; i < _allComboBoxItems.Count; i++)
			{
				var isAvailable = _allComboBoxItems[i].DataContext.containsFilterInOrder(_searchTextBox.Text, DisplayMemberPath);
				_allComboBoxItems[i].Visibility = isAvailable ? Visibility.Visible : Visibility.Collapsed;

				if (isAvailable)
					_visibleComboBoxItems.Add(i, _allComboBoxItems[i]);
			}

			var children = _hostPanel.VisualChildren().OfType<GroupItem>().ToList();

			foreach (var groupItem in children)
			{
				var hasVisibleChildren = groupItem
					.VisualChildren()
					.OfType<ComboBoxItem>()
					.Any(x => x.Visibility == Visibility.Visible);

				groupItem.Visibility = hasVisibleChildren ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void acceptValue()
		{
			var filteredChildren = _hostPanel
				.VisualChildren()
				.OfType<ComboBoxItem>()
				.Select((x, i) => new { index = i, cbItem = x })
				.Where(x => x.cbItem.IsVisible)
				.ToList();

			var firstHighlightedIndex = filteredChildren.FirstOrDefault(x => x.cbItem.IsHighlighted)?.index ?? -1;

			if (!string.IsNullOrWhiteSpace(_searchTextBox.Text) && filteredChildren.Count > 0)
				SelectedIndex = firstHighlightedIndex > 0 ? firstHighlightedIndex : filteredChildren.First().index;
		}

		private void onPopupOpened(object sender, EventArgs e)
		{
			_searchTextBox.Text = string.Empty;

			var items = _hostPanel.VisualChildren().OfType<ComboBoxItem>().ToList();

			for (var i = 0; i < items.Count; i++)
			{
				var item = items[i];

				if (_allComboBoxItems.Contains(item)) continue;

				item.MouseMove += onItemMouseEnter;

				_allComboBoxItems.Add(item);
				_visibleComboBoxItems.Add(i, item);
			}
		}

		private void onItemMouseEnter(object sender, MouseEventArgs e)
		{
			var comboBoxItem = sender as ComboBoxItem;

			if (comboBoxItem == null) return;

			SelectedItem = comboBoxItem.DataContext;
		}

		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);

			foreach (var item in _allComboBoxItems.Where(x => x.Background == Brushes.Red))
				item.Background = Brushes.Transparent;

			var selectedComboBoxItem = _allComboBoxItems.FirstOrDefault(x => x.DataContext == SelectedItem);

			if (selectedComboBoxItem != null)
				selectedComboBoxItem.Background = Brushes.Red;

			if (_editableTextBox == null) return;

			_editableTextBox.Text = SelectedValue?.ToString() ?? string.Empty;
		}
		#endregion
	}
}
