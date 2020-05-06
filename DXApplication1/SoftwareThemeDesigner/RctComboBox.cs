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
		private TextBox _displayTextBox;
		private ItemsPresenter _hostPanel;
		private ToggleButton _toggleButton;
		private RctRippleDecorator _rippleDecorator;
		#endregion

		#region Dependency Property Keys
		private static readonly DependencyPropertyKey valueTextPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(valueText), typeof(string), typeof(RctComboBox),
			new FrameworkPropertyMetadata(null));
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty labelTextProperty = DependencyProperty.Register(nameof(labelText), typeof(string), typeof(RctComboBox));
		public static readonly DependencyProperty labelFontSizeProperty = DependencyProperty.Register(nameof(labelFontSize), typeof(double), typeof(RctComboBox),
			new FrameworkPropertyMetadata(14d));
		public static readonly DependencyProperty labelTextColorProperty = DependencyProperty.Register(nameof(labelTextColor), typeof(Brush), typeof(RctComboBox));
		public static readonly DependencyProperty allowDeleteProperty = DependencyProperty.Register(nameof(allowDelete), typeof(bool), typeof(RctComboBox),
			new FrameworkPropertyMetadata(true));
		public static readonly DependencyProperty selectionBrushProperty = DependencyProperty.Register(nameof(selectionBrush), typeof(Brush), typeof(RctComboBox),
			new FrameworkPropertyMetadata(Brushes.DarkCyan));
		public static readonly DependencyProperty valueProperty = DependencyProperty.Register(nameof(value), typeof(object), typeof(RctComboBox),
			new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty valueTextProperty = valueTextPropertyKey.DependencyProperty;
		#endregion

		#region Routed Events
		public static readonly RoutedEvent valueChangedEvent = EventManager.RegisterRoutedEvent(nameof(valueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RctComboBox));
		#endregion

		#region Delegates
		public event RoutedEventHandler valueChanged
		{
			add { AddHandler(valueChangedEvent, value); }
			remove { RemoveHandler(valueChangedEvent, value); }
		}
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
		public Brush selectionBrush
		{
			get { return (Brush)GetValue(selectionBrushProperty); }
			set { SetValue(selectionBrushProperty, value); }
		}
		public object value
		{
			get { return GetValue(valueProperty); }
			set
			{
				SetValue(valueProperty, value);
				valueText = value.getObjectMember(DisplayMemberPath);
				RaiseEvent(new RoutedEventArgs(valueChangedEvent, value));
			}
		}
		public string valueText
		{
			get { return GetValue(valueTextProperty)?.ToString(); }
			protected set { SetValue(valueTextPropertyKey, value); }
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

			_displayTextBox = GetTemplateChild("PART_DisplayTextBox") as TextBox;
			if (_displayTextBox != null)
			{
				_displayTextBox.Focusable = false;
			}

			_searchTextBox = GetTemplateChild("PART_SearchTextBox") as TextBox;
			if (_searchTextBox != null)
			{
				_searchTextBox.Visibility = Visibility.Collapsed;
				_searchTextBox.TextChanged += onSearchBoxTextChanged;
			}

			_popup = GetTemplateChild("PART_Popup") as Popup;
			if (_popup != null)
			{
				_popup.Opened += onPopupOpened;
				_popup.Closed += onPopupClosed;
			}

			if (GetValue(labelTextColorProperty) as Brush == null)
				SetValue(labelTextColorProperty, Foreground);

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
				{
					if (_searchTextBox.IsFocused)
						SelectedItem = _visibleComboBoxItems.Values.FirstOrDefault()?.DataContext;

					acceptValue();
				}
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

			SelectedItem = null;

			if (!IsDropDownOpen)
				IsDropDownOpen = true;

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
			value = SelectedItem;
		}

		private void onPopupOpened(object sender, EventArgs e)
		{
			_displayTextBox.Visibility = Visibility.Collapsed;
			_searchTextBox.Visibility = Visibility.Visible;

			_searchTextBox.Text = string.Empty;

			var items = _hostPanel.VisualChildren().OfType<ComboBoxItem>().ToList();

			for (var i = 0; i < items.Count; i++)
			{
				var item = items[i];

				if (_allComboBoxItems.Contains(item)) continue;

				item.MouseMove += onItemMouseEnter;
				item.PreviewMouseUp += onItemMouseUp;

				_allComboBoxItems.Add(item);
				_visibleComboBoxItems.Add(i, item);
			}
		}

		private void onPopupClosed(object sender, EventArgs e)
		{
			_searchTextBox.Visibility = Visibility.Collapsed;
			_displayTextBox.Visibility = Visibility.Visible;
			_displayTextBox.Text = valueText;
		}

		private void onItemMouseEnter(object sender, MouseEventArgs e)
		{
			var comboBoxItem = sender as ComboBoxItem;

			if (comboBoxItem == null) return;

			SelectedItem = comboBoxItem.DataContext;
		}

		private void onItemMouseUp(object sender, MouseButtonEventArgs e)
		{
			IsDropDownOpen = false;
			acceptValue();
		}

		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);

			foreach (var item in _allComboBoxItems.Where(x => x.Background == selectionBrush))
				item.Background = Brushes.Transparent;

			var selectedComboBoxItem = _allComboBoxItems.FirstOrDefault(x => x.DataContext == SelectedItem);

			if (selectedComboBoxItem != null)
				selectedComboBoxItem.Background = selectionBrush;

			if (_displayTextBox == null) return;

			_displayTextBox.Text = SelectedValue?.ToString() ?? string.Empty;
		}
		#endregion
	}
}
