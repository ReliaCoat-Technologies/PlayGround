using System.Windows;
using EllipseDrawer.ViewModels;
using ReliaCoat.Common.UI.Extensions.SciChartExtensions;

namespace EllipseDrawer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();

            SciChartThemeManager.instance.setTheme(sciChartSurface);
        }
    }
}
