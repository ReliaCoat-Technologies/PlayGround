using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;

namespace GridDrag
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<UIElement> _gridElements;
        #endregion

        #region Properties
        public ObservableCollection<UIElement> gridElements
        {
            get { return _gridElements; }
            set { _gridElements = value; RaisePropertyChanged(() => gridElements); }
        }
        #endregion

        #region Commands
        public ICommand addItemCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            gridElements = new ObservableCollection<UIElement>();

            addItemCommand = new DelegateCommand(addItem);
        }
        #endregion

        #region Methods
        public void addItem()
        {
            var textblock = new TextBlock
            {
                Text = "Hello World!",
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            gridElements.Add(textblock);
        }
        #endregion
    }
}