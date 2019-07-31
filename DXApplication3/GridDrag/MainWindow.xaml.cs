using System;
using DevExpress.Xpf.Core;

namespace GridDrag
{
    public partial class MainWindow : DXWindow
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
