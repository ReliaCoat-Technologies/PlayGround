using System;
using System.Windows;

namespace GridDrag
{
    public partial class MainWindow : Window
    {
        #region ViewModels
        private MainWindowViewModel _viewModel;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }
    }
}
