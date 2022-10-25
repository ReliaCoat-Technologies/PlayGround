using System.Windows;

namespace ReliaCoat.GpuDetectorNetFramework
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}
