using SciChart.Charting.Model.DataSeries;
using System.Linq;
using System.Timers;
using System.Windows;

namespace DAQViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public XyDataSeries<double, float> dataBlue { get; set; }
        public XyDataSeries<double, float> dataOrange { get; set; }
        public DAQData daqData { get; set; }
        public Timer timer { get; set; }

        public const double dt = 0.10;
        private double time;

        public MainWindow()
        {
            InitializeComponent();

            dataBlue = new XyDataSeries<double, float>();
            dataOrange = new XyDataSeries<double, float>();

            daqData = new DAQData();
            daqData.armDAQ();
            tempSeries.DataSeries = dataBlue;
            voltSeries.DataSeries = dataOrange;

            timer = new Timer(dt * 1000);
            timer.Elapsed += Timer_Elapsed; ;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            daqData.ExtractData();
            dataBlue.Append(time, daqData.data1.Max());
            dataOrange.Append(time, daqData.data2.Max());
            time += dt;
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void GetData(object sender, RoutedEventArgs e)
        {
            Timer_Elapsed(null, null);
        }
    }
}
