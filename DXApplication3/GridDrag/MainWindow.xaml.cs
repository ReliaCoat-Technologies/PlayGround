using System;
using System.Windows;
using System.Windows.Controls;
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
            var textblock = new TextBlock
            {
                Text = "Hello World!",
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            gridStack.children.Add(textblock);
        }
    }
}
