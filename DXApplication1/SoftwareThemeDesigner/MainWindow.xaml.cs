using System;
using DevExpress.Xpf.Core;
using SoftwareThemeDesigner.RoutedEvents;
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

			spinBox.spin += onSpin;
        }

        private void onSpin(object sender, SpinRoutedEventArgs e)
        {
	        Console.WriteLine($"Spin Event Handled: Old Value = {e.oldValue}, New Value = {e.newValue}");
        }
    }
}
