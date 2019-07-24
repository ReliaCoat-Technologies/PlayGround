using System;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using EnumRadioButtonDesigner.Model;

namespace EnumRadioButtonDesigner
{
    public partial class MainWindow : DXWindow, INotifyPropertyChanged
    {
        private object _testEnumValue;

        #region Fields

        #endregion

        #region Delegates
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public object testEnumValue
        {
            get { return _testEnumValue; }
            set { _testEnumValue = value; raisePropertyChanged(nameof(testEnumValue)); }
        }
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void raisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
