using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;

namespace GridDrag.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<UIElement> _uiElementCollection;
        #endregion

        #region Properties
        public ObservableCollection<UIElement> uiElementCollection
        {
            get { return _uiElementCollection; }
            set { _uiElementCollection = value; }
        }
        #endregion

        #region Commands
        public ICommand addItemCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            uiElementCollection = new ObservableCollection<UIElement>();

            addItemCommand = new DelegateCommand(addItem);
        }
        #endregion

        #region Methods
        private void addItem()
        {
            var textblock = new TextBlock
            {
                Text = "Hello World!",
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            uiElementCollection.Add(textblock);
        }
        #endregion
    }
}