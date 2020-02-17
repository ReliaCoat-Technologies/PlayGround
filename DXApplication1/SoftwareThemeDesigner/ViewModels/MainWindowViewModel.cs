using System;
using DevExpress.Mvvm;

namespace SoftwareThemeDesigner.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private string _textBoxString;
        #endregion

        #region Properties
        public string textBoxString
        {
            get { return _textBoxString; }
            set { _textBoxString = value; RaisePropertyChanged(() => textBoxString); }
        }
        #endregion
    }
}