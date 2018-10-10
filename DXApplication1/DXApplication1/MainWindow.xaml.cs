using System;
using DevExpress.Xpf.Core;
using DXApplication1.ViewModels;

namespace DXApplication1
{
    public partial class MainWindow : DXWindow
    {
        public WindowViewModel windowViewModel { get; set; }

        public MainWindow()
        {
            windowViewModel = new WindowViewModel();
            DataContext = windowViewModel;

            InitializeComponent();
        }
    }
}
