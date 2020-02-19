using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SoftwareThemeDesigner
{
	public class RctComboBox : ComboBox
	{
		#region Fields
		private Popup popup;
		private TextBox searchTextBox;
		private TextBox editableTextBox;
		private StackPanel hostPanel;
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty labelTextProperty = DependencyProperty.Register(nameof(labelText), typeof(string), typeof(RctComboBox));
		public static readonly DependencyProperty labelFontSizeProperty = DependencyProperty.Register(nameof(labelFontSize), typeof(double), typeof(RctComboBox),
			new FrameworkPropertyMetadata(12d));
		public static readonly DependencyProperty labelTextColorProperty = DependencyProperty.Register(nameof(labelTextColor), typeof(Brush), typeof(RctComboBox));
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
		#endregion

		#region Constructor
		static RctComboBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctComboBox), new FrameworkPropertyMetadata(typeof(RctComboBox)));
			FontSizeProperty.OverrideMetadata(typeof(RctComboBox), new FrameworkPropertyMetadata(24d));
			BorderBrushProperty.OverrideMetadata(typeof(RctComboBox), new FrameworkPropertyMetadata(Brushes.White));
		}
		#endregion

		#region Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			hostPanel = GetTemplateChild("PART_HostPanel") as StackPanel;

			editableTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
			if (editableTextBox != null)
			{
				editableTextBox.Focusable = false;
			}

			searchTextBox = GetTemplateChild("PART_SearchTextBox") as TextBox;
			if (searchTextBox != null)
			{
				searchTextBox.TextChanged += onSearchCriteriaChanged;
			}

			popup = GetTemplateChild("PART_Popup") as Popup;
			if (popup != null)
			{
				popup.Focusable = true;
			}

			var label = GetTemplateChild("PART_Label") as TextBlock;
			if (label != null)
			{
				if (labelTextColor == null)
					labelTextColor = Foreground;

				label.Text = labelText;
				label.FontSize = labelFontSize;
				label.Foreground = labelTextColor;
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				searchTextBox.Text = string.Empty;
			}
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				acceptValue();
			}
			if (e.Key == Key.Up || e.Key == Key.Down)
			{
				IsDropDownOpen = true;
				return;
			}

			searchTextBox.Focus();

			base.OnPreviewKeyDown(e);
		}

		private void onSearchCriteriaChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(searchTextBox.Text))
			{
				Items.Filter = null;
				return;
			}

			Items.Filter = x => x.ToString().ToLower().Contains(searchTextBox.Text.ToLower());
		}

		private void acceptValue()
		{
			if (!string.IsNullOrWhiteSpace(searchTextBox.Text) && Items.Count > 0)
				SelectedItem = Items[0];

			searchTextBox.Text = string.Empty;
		}
		#endregion
	}
}
