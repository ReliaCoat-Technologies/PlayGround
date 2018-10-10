using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DXApplication1
{
    public partial class SpecScaleBar : UserControl
    {
        #region Constants
        public const string LowerFailLimitPropertyName = "LowerFailLimit";
        public const string LowerWarnLimitPropertyName = "LowerWarnLimit";
        public const string SpecValuePropertyName = "SpecValue";
        public const string UpperWarnLimitPropertyName = "UpperWarnLimit";
        public const string UpperFailLimitPropertyName = "UpperFailLimit";
        public const string SpecLimitPaddingPropertyName = "SpecLimitPadding";
        public const string MeasuredValuePropertyName = "MeasuredValue";
        public const string PassColorPropertyName = "PassColor";
        public const string WarnColorPropertyName = "WarnColor";
        public const string FailColorPropertyName = "FailColor";
        public const string ForegroundPropertyName = "Foreground";
        public const string FontSizePropertyName = "FontSize";
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty LowerFailLimitProperty = DependencyProperty.Register(LowerFailLimitPropertyName, typeof(double?), typeof(SpecScaleBar), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty LowerWarnLimitProperty = DependencyProperty.Register(LowerWarnLimitPropertyName, typeof(double?), typeof(SpecScaleBar), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty SpecValueProperty = DependencyProperty.Register(SpecValuePropertyName, typeof(double?), typeof(SpecScaleBar), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty UpperWarnLimitProperty = DependencyProperty.Register(UpperWarnLimitPropertyName, typeof(double?), typeof(SpecScaleBar), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty UpperFailLimitProperty = DependencyProperty.Register(UpperFailLimitPropertyName, typeof(double?), typeof(SpecScaleBar), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty SpecLimitPaddingProperty = DependencyProperty.Register(SpecLimitPaddingPropertyName, typeof(double?), typeof(SpecScaleBar), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty MeasuredValueProperty = DependencyProperty.Register(MeasuredValuePropertyName, typeof(double?), typeof(SpecScaleBar), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty PassColorProperty = DependencyProperty.Register(PassColorPropertyName, typeof(SolidColorBrush), typeof(SpecScaleBar), new PropertyMetadata(new SolidColorBrush(Colors.Green), PropertyChangedCallback));
        public static readonly DependencyProperty WarnColorProperty = DependencyProperty.Register(WarnColorPropertyName, typeof(SolidColorBrush), typeof(SpecScaleBar), new PropertyMetadata(new SolidColorBrush(Colors.Yellow), PropertyChangedCallback));
        public static readonly DependencyProperty FailColorProperty = DependencyProperty.Register(FailColorPropertyName, typeof(SolidColorBrush), typeof(SpecScaleBar), new PropertyMetadata(new SolidColorBrush(Colors.Red), PropertyChangedCallback));
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(ForegroundPropertyName, typeof(SolidColorBrush), typeof(SpecScaleBar), new PropertyMetadata(new SolidColorBrush(Colors.White), PropertyChangedCallback));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(FontSizePropertyName, typeof(double), typeof(SpecScaleBar), new UIPropertyMetadata(12d, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var specScaleBar = d as SpecScaleBar;
            if (specScaleBar == null) return;

            switch (e.Property.Name)
            {
                case LowerFailLimitPropertyName:
                    specScaleBar._lowerFailLimit = e.NewValue as double?;
                    break;
                case LowerWarnLimitPropertyName:
                    specScaleBar._lowerWarnLimit = e.NewValue as double?;
                    break;
                case SpecValuePropertyName:
                    specScaleBar._specValue = e.NewValue as double?;
                    break;
                case UpperWarnLimitPropertyName:
                    specScaleBar._upperWarnLimit = e.NewValue as double?;
                    break;
                case UpperFailLimitPropertyName:
                    specScaleBar._upperFailLimit = e.NewValue as double?;
                    break;
                case SpecLimitPaddingPropertyName:
                    specScaleBar._scaleBarPadding = e.NewValue as double?;
                    break;
                case MeasuredValuePropertyName:
                    specScaleBar._measuredValue = e.NewValue as double?;
                    break;
                case PassColorPropertyName:
                    specScaleBar._passColor = (SolidColorBrush)e.NewValue;
                    break;
                case WarnColorPropertyName:
                    specScaleBar._warnColor = (SolidColorBrush)e.NewValue;
                    break;
                case FailColorPropertyName:
                    specScaleBar._failColor = (SolidColorBrush)e.NewValue;
                    break;
                case ForegroundPropertyName:
                    specScaleBar._foreground = (SolidColorBrush)e.NewValue;
                    break;
                case FontSizePropertyName:
                    specScaleBar._fontSize = (double)e.NewValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Property {e.Property.Name} change not handled");
            }

            specScaleBar.updateScales();
        }
        #endregion

        #region Fields
        private double? _lowerFailLimit;
        private double? _lowerWarnLimit;
        private double? _specValue;
        private double? _upperWarnLimit;
        private double? _upperFailLimit;
        private double? _scaleBarPadding;
        private double? _measuredValue;

        private SolidColorBrush _passColor;
        private SolidColorBrush _warnColor;
        private SolidColorBrush _failColor;
        private SolidColorBrush _foreground;
        private double _fontSize;

        private double _specMaximum;
        private double _specMinimum;
        private double _specSpan;
        private double _padValue;
        private double _scaleBarMinimum;
        private double _scaleBarMaximum;
        private double _scaleBarSpan;
        private double _scaleToValueRatio;

        private double _lowerFailWidth;
        private double _lowerWarnWidth;
        private double _lowerPassWidth;
        private double _upperPassWidth;
        private double _upperWarnWidth;
        private double _upperFailWidth;
        private double _measureWidthLeft;
        private double _measureWidthRight;
        private double _leftSideMarkerPadding;
        private double _rightSideMarkerPadding;
        #endregion

        #region Properties
        public double? LowerFailLimit
        {
            get { return (double?)GetValue(LowerFailLimitProperty); }
            set { SetValue(LowerFailLimitProperty, value); }
        }
        public double? LowerWarnLimit
        {
            get { return (double?)GetValue(LowerWarnLimitProperty); }
            set { SetValue(LowerWarnLimitProperty, value); }
        }
        public double? SpecValue
        {
            get { return (double?)GetValue(SpecValueProperty); }
            set { SetValue(SpecValueProperty, value); }
        }
        public double? UpperWarnLimit
        {
            get { return (double?)GetValue(UpperWarnLimitProperty); }
            set { SetValue(UpperWarnLimitProperty, value); }
        }
        public double? UpperFailLimit
        {
            get { return (double?)GetValue(UpperFailLimitProperty); }
            set { SetValue(UpperFailLimitProperty, value); }
        }
        public double? SpecLimitPadding
        {
            get { return (double?)GetValue(SpecLimitPaddingProperty); }
            set { SetValue(SpecLimitPaddingProperty, value); }
        }
        public double? MeasuredValue
        {
            get { return (double?)GetValue(MeasuredValueProperty); }
            set { SetValue(MeasuredValueProperty, value); }
        }
        public SolidColorBrush PassColor
        {
            get { return (SolidColorBrush)GetValue(PassColorProperty); }
            set { SetValue(PassColorProperty, value); }
        }
        public SolidColorBrush WarnColor
        {
            get { return (SolidColorBrush)GetValue(WarnColorProperty); }
            set { SetValue(WarnColorProperty, value); }
        }
        public SolidColorBrush FailColor
        {
            get { return (SolidColorBrush)GetValue(FailColorProperty); }
            set { SetValue(FailColorProperty, value); }
        }
        public new SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        #endregion

        #region Constructor
        public SpecScaleBar()
        {
            InitializeComponent();
            resetChart();
        }
        #endregion

        #region Methods
        private void updateScales()
        {
            resetChart();

            if (_specValue == null) return;
            if (_scaleBarPadding == null) _scaleBarPadding = 0.1;

            if (!setMarkersAndWidths()) return;
            setSpecTextBlocks();
            setMeasuredValues();
        }

        private void setSpecTextBlocks()
        {
            lowerFailLowerSpan.Stroke = _failColor;
            lowerFailUpperSpan.Stroke = _lowerWarnWidth == 0 ? _passColor : _warnColor;
            lowerWarnLowerSpan.Stroke = _warnColor;
            lowerWarnUpperSpan.Stroke = _passColor;
            specMarkerLowerSpan.Stroke = _passColor;
            specMarkerUpperSpan.Stroke = _passColor;
            upperWarnLowerSpan.Stroke = _passColor;
            upperWarnUpperSpan.Stroke = _warnColor;
            upperFailLowerSpan.Stroke = _upperWarnWidth == 0 ? _passColor : _warnColor;
            upperFailUpperSpan.Stroke = _failColor;

            lowerFailLimitTextBlock.Text = _lowerFailLimit.ToString();
            lowerWarnLimitTextBlock.Text = _lowerWarnLimit.ToString();
            specValueTextBlock.Text = _specValue.ToString();
            upperWarnLimitTextBlock.Text = _upperWarnLimit.ToString();
            upperFailLimitTextBlock.Text = _upperFailLimit.ToString();
        }

        private void resetChart()
        {
            if (_fontSize <= 0) _fontSize = 12;

            lowFailBar.Stroke = highFailBar.Stroke = _failColor;
            lowWarnBar.Stroke = highWarnBar.Stroke = _warnColor;
            lowPassBar.Stroke = highPassBar.Stroke = _passColor;

            _lowerFailWidth = 0;
            _lowerWarnWidth = 0;
            _lowerPassWidth = 0;
            _upperPassWidth = 0;
            _upperWarnWidth = 0;
            _upperFailWidth = 0;

            lowerFailLowerSpan.Stroke = _failColor;
            lowerFailUpperSpan.Stroke = _warnColor;
            lowerWarnLowerSpan.Stroke = _warnColor;
            lowerWarnUpperSpan.Stroke = _passColor;
            specMarkerLowerSpan.Stroke = _passColor;
            specMarkerUpperSpan.Stroke = _passColor;
            upperWarnLowerSpan.Stroke = _passColor;
            upperWarnUpperSpan.Stroke = _warnColor;
            upperFailLowerSpan.Stroke = _warnColor;
            upperFailUpperSpan.Stroke = _failColor;

            lowerFailMarker.Stroke = _foreground;
            lowerWarnMarker.Stroke = _foreground;
            specMarker.Stroke = _foreground;
            upperWarnMarker.Stroke = _foreground;
            upperFailMarker.Stroke = _foreground;
            lowerLimitArrow.Stroke = _foreground;
            upperLimitArrow.Stroke = _foreground;
            measuredValueEllipse.Stroke = _foreground;

            lowerFailLimitTextBlock.Foreground = _foreground;
            lowerWarnLimitTextBlock.Foreground = _foreground;
            specValueTextBlock.Foreground = _foreground;
            upperWarnLimitTextBlock.Foreground = _foreground;
            upperFailLimitTextBlock.Foreground = _foreground;

            lowerFailLimitTextBlock.FontSize = _fontSize;
            lowerWarnLimitTextBlock.FontSize = _fontSize;
            specValueTextBlock.FontSize = _fontSize;
            upperWarnLimitTextBlock.FontSize = _fontSize;
            upperFailLimitTextBlock.FontSize = _fontSize;

            measuredValueEllipse.Visibility = Visibility.Hidden;
            lowerLimitArrow.Visibility = Visibility.Hidden;
            upperLimitArrow.Visibility = Visibility.Hidden;
        }

        private void setDefaultValues()
        {
            _lowerFailWidth = 10;
            _lowerWarnWidth = 10;
            _lowerPassWidth = 10;
            _upperPassWidth = 10;
            _upperWarnWidth = 10;
            _upperFailWidth = 10;

            _scaleToValueRatio = 100d / 60d;

            measuredValueEllipse.Visibility = Visibility.Hidden;
            lowerLimitArrow.Visibility = Visibility.Hidden;
            upperLimitArrow.Visibility = Visibility.Hidden;

            lowerFailLimitTextBlock.Text = string.Empty;
            lowerWarnLimitTextBlock.Text = string.Empty;
            specValueTextBlock.Text = string.Empty;
            upperWarnLimitTextBlock.Text = string.Empty;
            upperFailLimitTextBlock.Text = string.Empty;
        }

        private bool setMarkersAndWidths()
        {
            if (_specValue == null) return false;
            if (_scaleBarPadding == null) _scaleBarPadding = 0.1;

            bool setLimitsSuccessful;

            if (_lowerFailLimit == null && _upperFailLimit == null)
            {
                setLimitsSuccessful = setNeitherLimits();
            }
            else if (_lowerFailLimit == null)
            {
                setLimitsSuccessful = setUpperLimits();
            }
            else if (_upperFailLimit == null)
            {
                setLimitsSuccessful = setLowerLimits();
            }
            else
            {
                setLimitsSuccessful = setBothLimits();
            }

            if (!setLimitsSuccessful)
                setDefaultValues();

            setScaleSpan();

            if (_scaleBarSpan <= 0 || _scaleToValueRatio <= 0)
            {
                setLimitsSuccessful = false;
                setDefaultValues();
            }

            setChartWidths();

            return setLimitsSuccessful;
        }

        private bool setNeitherLimits()
        {
            _specMinimum = _specMaximum = _specValue.Value;
            _padValue = _specValue.Value * _scaleBarPadding.Value;
            _lowerPassWidth = _padValue;
            _upperPassWidth = _padValue;

            lowerWarnMarkerColumn.Width = new GridLength(0);
            lowerFailMarkerColumn.Width = new GridLength(0);
            upperFailMarkerColumn.Width = new GridLength(0);
            upperWarnMarkerColumn.Width = new GridLength(0);

            return true;
        }

        private void getSpanAndPadding(ref double lowerEdgeWidth, ref double upperEdgeWidth)
        {
            _specSpan = _specMaximum - _specMinimum;
            _padValue = _specSpan * _scaleBarPadding.Value;

            lowerEdgeWidth = _padValue;
            upperEdgeWidth = _padValue;
        }

        private bool setLowerLimits()
        {
            if (!setLowerWarnLimit()) return false;

            var extraPassWidth = _specValue.Value - _lowerFailLimit.Value;

            _specMinimum = _lowerFailLimit.Value;
            _specMaximum = _specValue.Value + extraPassWidth;
            getSpanAndPadding(ref _lowerFailWidth, ref _upperPassWidth);

            _upperPassWidth += extraPassWidth;

            lowerFailMarkerColumn.Width = GridLength.Auto;
            upperWarnMarkerColumn.Width = new GridLength(0);
            upperFailMarkerColumn.Width = new GridLength(0);

            return true;
        }

        private bool setUpperLimits()
        {
            if (!setUpperWarnLimit()) return false;

            var extraPassWidth = _upperFailLimit.Value - _specValue.Value;

            _specMinimum = _specValue.Value - extraPassWidth;
            _specMaximum = _upperFailLimit.Value;
            getSpanAndPadding(ref _lowerPassWidth, ref _upperFailWidth);

            _lowerPassWidth += extraPassWidth;

            lowerWarnMarkerColumn.Width = new GridLength(0);
            lowerFailMarkerColumn.Width = new GridLength(0);
            upperFailMarkerColumn.Width = GridLength.Auto;
            return true;
        }

        private bool setBothLimits()
        {
            if (!setLowerWarnLimit() || !setUpperWarnLimit()) return false;
            _specMinimum = _lowerFailLimit.Value;
            _specMaximum = _upperFailLimit.Value;
            getSpanAndPadding(ref _lowerFailWidth, ref _upperFailWidth);

            lowerFailMarkerColumn.Width = GridLength.Auto;
            upperFailMarkerColumn.Width = GridLength.Auto;
            return true;
        }

        private void setScaleSpan()
        {
            _scaleBarMinimum = _specMinimum - _padValue;
            _scaleBarMaximum = _specMaximum + _padValue;
            _scaleBarSpan = _scaleBarMaximum - _scaleBarMinimum;
            _scaleToValueRatio = 100 / _scaleBarSpan;
        }

        private void setChartWidths()
        {
            lowerFailWidth.Width = new GridLength(_lowerFailWidth * _scaleToValueRatio, GridUnitType.Star);
            lowerWarnWidth.Width = new GridLength(_lowerWarnWidth * _scaleToValueRatio, GridUnitType.Star);
            lowerPassWidth.Width = new GridLength(_lowerPassWidth * _scaleToValueRatio, GridUnitType.Star);
            upperPassWidth.Width = new GridLength(_upperPassWidth * _scaleToValueRatio, GridUnitType.Star);
            upperWarnWidth.Width = new GridLength(_upperWarnWidth * _scaleToValueRatio, GridUnitType.Star);
            upperFailWidth.Width = new GridLength(_upperFailWidth * _scaleToValueRatio, GridUnitType.Star);
        }

        private bool setLowerWarnLimit()
        {
            if (_lowerWarnLimit == null)
            {
                if (_specValue.Value < _lowerFailLimit.Value) return false;

                _lowerPassWidth = _specValue.Value - _lowerFailLimit.Value;
                _lowerWarnWidth = 0;
                lowerWarnMarkerColumn.Width = new GridLength(0);
            }
            else
            {
                if (_specValue.Value < _lowerWarnLimit.Value || _lowerFailLimit.Value > _lowerWarnLimit.Value)
                    return false;

                _lowerPassWidth = _specValue.Value - _lowerWarnLimit.Value;
                _lowerWarnWidth = _lowerWarnLimit.Value - _lowerFailLimit.Value;
                lowerWarnMarkerColumn.Width = GridLength.Auto;
            }

            return true;
        }

        private bool setUpperWarnLimit()
        {
            if (_upperWarnLimit == null)
            {
                _upperPassWidth = _upperFailLimit.Value - _specValue.Value;
                _upperWarnWidth = 0;
                upperWarnMarkerColumn.Width = new GridLength(0);
            }
            else
            {
                if (_specValue.Value > _upperWarnLimit.Value || _upperWarnLimit.Value > _upperFailLimit.Value)
                    return false;

                _upperPassWidth = _upperWarnLimit.Value - _specValue.Value;
                _upperWarnWidth = _upperFailLimit.Value - _upperWarnLimit.Value;
                upperWarnMarkerColumn.Width = GridLength.Auto;
            }

            return true;
        }


        private void setMeasuredValues()
        {
            if (_measuredValue == null)
            {
                measuredValueEllipse.Visibility = Visibility.Hidden;
                lowerLimitArrow.Visibility = Visibility.Hidden;
                upperLimitArrow.Visibility = Visibility.Hidden;
            }
            else if (_measuredValue.Value > _scaleBarMaximum)
            {
                measuredValueEllipse.Visibility = Visibility.Hidden;
                lowerLimitArrow.Visibility = Visibility.Hidden;

                upperLimitArrow.Visibility = Visibility.Visible;
                upperLimitArrow.Fill = getEvaluationFillColor();
            }
            else if (_measuredValue.Value < _scaleBarMinimum)
            {
                measuredValueEllipse.Visibility = Visibility.Hidden;
                upperLimitArrow.Visibility = Visibility.Hidden;

                lowerLimitArrow.Visibility = Visibility.Visible;
                lowerLimitArrow.Fill = getEvaluationFillColor();
            }
            else
            {
                measuredValueEllipse.Visibility = Visibility.Visible;
                measuredValueEllipse.Fill = getEvaluationFillColor();

                _measureWidthLeft = (_measuredValue.Value - _specMinimum + _padValue);
                _measureWidthRight = _scaleBarSpan - _measureWidthLeft;

                measureValueColumnLeft.Width = new GridLength(_measureWidthLeft * _scaleToValueRatio, GridUnitType.Star);
                measureValueColumnRight.Width = new GridLength(_measureWidthRight * _scaleToValueRatio, GridUnitType.Star);
            }
        }

        private SolidColorBrush getEvaluationFillColor()
        {
            var lowerFailLimit = _lowerFailLimit ?? double.NegativeInfinity;
            var lowerWarnLimit = _lowerWarnLimit ?? double.NegativeInfinity;
            var upperWarnLimit = _upperWarnLimit ?? double.PositiveInfinity;
            var upperFailLimit = _upperFailLimit ?? double.PositiveInfinity;

            if (_measuredValue <= lowerFailLimit || _measuredValue >= upperFailLimit)
            {
                return _failColor;
            }
            if (MeasuredValue <= lowerWarnLimit || _measuredValue >= upperWarnLimit)
            {
                return _warnColor;
            }

            return _passColor;
        }
        #endregion
    }
}
