using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Gauges;

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
            resetCharts();
        }
        #endregion

        #region Methods
        private void resetCharts()
        {
            lowerFailRange.StartValue = lowerFailRange.EndValue =
            lowerWarnRange.StartValue = lowerWarnRange.EndValue =
            lowerPassRange.StartValue = lowerPassRange.EndValue =
            upperPassRange.StartValue = upperPassRange.EndValue =
            upperWarnRange.StartValue = upperWarnRange.EndValue =
            upperFailRange.StartValue = upperFailRange.EndValue =
            new RangeValue(0, RangeValueType.Absolute);
        }

        private void updateScales()
        {
            resetCharts();

            if (_specValue == null) return;

            if (_scaleBarPadding == null || _scaleBarPadding < 0.1) _scaleBarPadding = 0.1;
            if (_scaleBarPadding > 0.5) _scaleBarPadding = 0.5;

            setRanges();

            scaleMarker.Value = _measuredValue ?? 0;
        }

        private void setRanges()
        {
            if (_specValue == null) return;
            if (_scaleBarPadding == null) return;

            double adjustedLowerWarnLimit, adjustedUpperWarnLimit;
            getWarnValues(out adjustedLowerWarnLimit, out adjustedUpperWarnLimit);

            if (_lowerFailLimit == null && _upperFailLimit == null)
            {
                _padValue = _specValue.Value * _scaleBarPadding.Value;

                _scaleBarMinimum = _specValue.Value - _padValue;
                _scaleBarMaximum = _specValue.Value + _padValue;

                lowerPassRange.StartValue = new RangeValue(_scaleBarMinimum, RangeValueType.Absolute);
                upperPassRange.EndValue = new RangeValue(_scaleBarMaximum, RangeValueType.Absolute);
            }
            else if (_lowerFailLimit == null && _upperFailLimit != null)
            {
                var passFailDifference = _upperFailLimit.Value - _specValue.Value;
                setSpanAndPadding(_specValue.Value - passFailDifference, _upperFailLimit.Value);

                lowerPassRange.StartValue = new RangeValue(_scaleBarMinimum);
                lowerPassRange.EndValue = upperPassRange.StartValue = new RangeValue(_specValue.Value, RangeValueType.Absolute);
                upperPassRange.EndValue = upperWarnRange.StartValue = new RangeValue(adjustedUpperWarnLimit, RangeValueType.Absolute);
                upperWarnRange.EndValue = upperFailRange.StartValue = new RangeValue(_upperFailLimit.Value, RangeValueType.Absolute);
                upperFailRange.EndValue = new RangeValue(_scaleBarMaximum);
            }
            else if (_lowerFailLimit != null && _upperFailLimit == null)
            {
                var passFailDifference = _specValue.Value - _lowerFailLimit.Value;
                setSpanAndPadding(_lowerFailLimit.Value, _specValue.Value + passFailDifference);

                lowerFailRange.StartValue = new RangeValue(_scaleBarMinimum);
                lowerFailRange.EndValue = lowerWarnRange.StartValue = new RangeValue(_lowerFailLimit.Value, RangeValueType.Absolute);
                lowerWarnRange.EndValue = lowerPassRange.StartValue = new RangeValue(adjustedLowerWarnLimit, RangeValueType.Absolute);
                lowerPassRange.EndValue = upperPassRange.StartValue = new RangeValue(_specValue.Value, RangeValueType.Absolute);
                upperPassRange.EndValue = new RangeValue(_scaleBarMaximum);
            }
            else if (_lowerFailLimit != null && _upperFailLimit != null)
            {
                setSpanAndPadding(_lowerFailLimit.Value, _upperFailLimit.Value);

                lowerFailRange.StartValue = new RangeValue(_scaleBarMinimum);
                lowerFailRange.EndValue = lowerWarnRange.StartValue = new RangeValue(_lowerFailLimit.Value, RangeValueType.Absolute);
                lowerWarnRange.EndValue = lowerPassRange.StartValue = new RangeValue(adjustedLowerWarnLimit, RangeValueType.Absolute);
                lowerPassRange.EndValue = upperPassRange.StartValue = new RangeValue(_specValue.Value, RangeValueType.Absolute);
                upperPassRange.EndValue = upperWarnRange.StartValue = new RangeValue(adjustedUpperWarnLimit, RangeValueType.Absolute);
                upperWarnRange.EndValue = upperFailRange.StartValue = new RangeValue(_upperFailLimit.Value, RangeValueType.Absolute);
                upperFailRange.EndValue = new RangeValue(_scaleBarMaximum);
            }

            linearScaleBar.StartValue = _scaleBarMinimum;
            linearScaleBar.EndValue = _scaleBarMaximum;
        }

        private void getWarnValues(out double adjustedLowerWarnLimit, out double adjustedUpperWarnLimit)
        {
            if (_specValue == null)
            {
                adjustedUpperWarnLimit = 0;
                adjustedLowerWarnLimit = 0;
                return;
            }

            if (_lowerFailLimit == null)
                adjustedLowerWarnLimit = 0;
            else if (_lowerWarnLimit == null || _lowerWarnLimit.Value < _lowerFailLimit.Value || _lowerWarnLimit.Value > _specValue.Value)
                adjustedLowerWarnLimit = _lowerFailLimit.Value;
            else
                adjustedLowerWarnLimit = _lowerWarnLimit.Value;

            if (_upperFailLimit == null)
                adjustedUpperWarnLimit = 0;
            else if (_upperWarnLimit == null || _upperWarnLimit.Value > _upperFailLimit.Value || _upperWarnLimit.Value < _specValue.Value)
                adjustedUpperWarnLimit = _upperFailLimit.Value;
            else
                adjustedUpperWarnLimit = _upperWarnLimit.Value;
        }

        private void setSpanAndPadding(double specMinimum, double specMaximum)
        {
            _specMinimum = specMinimum;
            _specMaximum = specMaximum;

            _specSpan = specMaximum - specMinimum;
            _padValue = _specSpan * _scaleBarPadding.Value;

            _scaleBarMinimum = _specMinimum - _padValue;
            _scaleBarMaximum = _specMaximum + _padValue;
        }

        #endregion
    }
}
