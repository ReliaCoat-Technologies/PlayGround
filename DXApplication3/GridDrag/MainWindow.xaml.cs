using System;
using System.Windows;
using DevExpress.Xpf.Core;

namespace GridDrag
{
    public partial class MainWindow : DXWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            gridStack.autoAddGridItem(2, 2);
        }
    }
}
