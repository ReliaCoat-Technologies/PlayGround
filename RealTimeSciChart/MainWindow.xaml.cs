using SciChart.Charting.Model.DataSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RealTimeSciChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const double dt = 0.05;

        public Timer timer;

        public double t;

        private IXyDataSeries<double, double> _series0;
        private IXyDataSeries<double, double> _series1;
        private IXyDataSeries<double, double> _series2;

        public MainWindow()
        {
            InitializeComponent();

            _series0 = new XyDataSeries<double, double>() { SeriesName = "Orange Series" };
            _series1 = new XyDataSeries<double, double>() { SeriesName = "Blue Series" };
            _series2 = new XyDataSeries<double, double>() { SeriesName = "Green Series" };

            timer = new Timer(dt * 1000);
            timer.AutoReset = true;
            timer.Elapsed += OnNewData;

            series0.DataSeries = _series0;
            series1.DataSeries = _series1;
            series2.DataSeries = _series2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void OnNewData(object sender, ElapsedEventArgs e)
        {
            double y1 = 3.0 * Math.Sin(((2 * Math.PI) * 1.4) * t * 0.02);
            double y2 = 2.0 * Math.Cos(((2 * Math.PI) * 0.8) * t * 0.02);
            double y3 = 1.0 * Math.Sin(((2 * Math.PI) * 2.2) * t * 0.02);

            using (chart.SuspendUpdates())
            {
                _series0.Append(t, y1);
                _series1.Append(t, y2);
                _series2.Append(t, y3);
            }

            t += dt;
        }
    }
}
