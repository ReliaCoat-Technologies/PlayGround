using System;
using DevExpress.Xpf.Core;
using SoftwareThemeDesigner.ViewModels;

namespace SoftwareThemeDesigner
{
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
