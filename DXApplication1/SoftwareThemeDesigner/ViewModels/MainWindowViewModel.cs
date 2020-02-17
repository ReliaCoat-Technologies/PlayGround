using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;

namespace SoftwareThemeDesigner.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private string _textBoxString;
        private ObservableCollection<string> _comboBoxItems;
        private string _comboBoxValue;
        #endregion

        #region Properties
        public string textBoxString
        {
            get { return _textBoxString; }
            set { _textBoxString = value; RaisePropertyChanged(() => textBoxString); }
        }
        public ObservableCollection<string> comboBoxItems
        {
            get { return _comboBoxItems; }
            set { _comboBoxItems = value; RaisePropertyChanged(() => comboBoxItems); }
        }
        public string comboBoxValue
        {
            get { return _comboBoxValue; }
            set { _comboBoxValue = value; RaisePropertyChanged(() => comboBoxValue); }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            var itemsToAdd = Enumerable.Range(0, 1000)
                .Select(x => x.ToString());

            comboBoxItems = new ObservableCollection<string>(itemsToAdd);
        }
        #endregion
    }
}