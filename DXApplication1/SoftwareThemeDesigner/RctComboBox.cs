using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SoftwareThemeDesigner
{
    public class RctComboBox : ComboBox
    {
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

            SelectionChanged += OnSelectionChanged;

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

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = e.AddedItems.OfType<object>().FirstOrDefault();
        }
        #endregion
    }
}
