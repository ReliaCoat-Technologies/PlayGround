using System;
using DevExpress.Xpf.Core;
using System.Windows;

namespace GridDrag
{
    public partial class MainWindow : DXWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            gridStack.addGridItem(0, 0, 2, 2);
            gridStack.addGridItem(2, 4, 2, 2);
        }

        private void onAddRowClicked(object sender, RoutedEventArgs e)
        {
            gridStack.addRowDefinition();
        }
    }
}
