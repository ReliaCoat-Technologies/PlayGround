using System;
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
		private Popup _popup;
		private TextBlock _label;
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
		#endregion

		#region Constructor
		static RctComboBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctComboBox), new FrameworkPropertyMetadata(typeof(RctComboBox)));
			FontSizeProperty.OverrideMetadata(typeof(RctComboBox), new FrameworkPropertyMetadata(24d));
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
				IsDropDownOpen = true;

				return;
			}

			_searchTextBox.Focus();

			base.OnPreviewKeyDown(e);
		}

		private void onSearchBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			foreach (var item in _hostPanel.VisualChildren().OfType<ComboBoxItem>())
			{
				var isAvailable = item.Content.containsFilterInOrder(_searchTextBox.Text, DisplayMemberPath);
				item.Visibility = isAvailable ? Visibility.Visible : Visibility.Collapsed;
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
		}

		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);

			if (_editableTextBox == null) return;

			_editableTextBox.Text = SelectedValue?.ToString() ?? string.Empty;
		}
		#endregion
	}
}
