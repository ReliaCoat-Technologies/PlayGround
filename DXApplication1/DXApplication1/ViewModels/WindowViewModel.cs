using System;
using DevExpress.Mvvm;

namespace DXApplication1.ViewModels
{
    public class WindowViewModel : ViewModelBase
    {
        private double? _lowerSpecLimit;
        private double? _lowerWarnLimit;
        private double? _specValue;
        private double? _upperWarnLimit;
        private double? _upperSpecLimit;
        private double? _measuredValue;
        private double? _specLimitPadding;

        public double? lowerSpecLimit
        {
            get { return _lowerSpecLimit; }
            set { _lowerSpecLimit = value; RaisePropertyChanged(() => lowerSpecLimit); }
        }
        public double? lowerWarnLimit
        {
            get { return _lowerWarnLimit; }
            set { _lowerWarnLimit = value; RaisePropertyChanged(() => lowerWarnLimit); }
        }
        public double? specValue
        {
            get { return _specValue; }
            set { _specValue = value; RaisePropertyChanged(() => specValue); }
        }
        public double? upperWarnLimit
        {
            get { return _upperWarnLimit; }
            set { _upperWarnLimit = value; RaisePropertyChanged(() => upperWarnLimit); }
        }
        public double? upperSpecLimit
        {
            get { return _upperSpecLimit; }
            set { _upperSpecLimit = value; RaisePropertyChanged(() => upperSpecLimit); }
        }
        public double? measuredValue
        {
            get { return _measuredValue; }
            set { _measuredValue = value; RaisePropertyChanged(() => measuredValue); }
        }
        public double? specLimitPadding
        {
            get { return _specLimitPadding; }
            set { _specLimitPadding = value; RaisePropertyChanged(() => specLimitPadding); }
        }
    }
}