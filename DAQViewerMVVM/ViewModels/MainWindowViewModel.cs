using System;
using DevExpress.Mvvm;
using SciChart.Charting.Model.DataSeries;
using DAQViewer;
using System.Timers;
using System.Linq;

namespace DAQViewerMVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        XyDataSeries<double, float> _dataBlue;
        XyDataSeries<double, float> _dataOrange;

        public DelegateCommand startCommand { get; set; }
        public DelegateCommand stopCommand { get; set; }
        public DelegateCommand getDataCommand { get; set; }

        public XyDataSeries<double, float> dataBlue
        {
            get { return _dataBlue; }
            set
            {
                _dataBlue = value;
                RaisePropertyChanged("dataBlue");
            }
        }
        public XyDataSeries<double, float> dataOrange
        {
            get { return _dataOrange; }
            set
            {
                _dataOrange = value;
                RaisePropertyChanged("dataOrange");
            }
        }

        public DAQData daqData;
        public Timer timer;
        public const double dt = 0.10;
        private double time;

        public MainWindowViewModel()
        {
            dataBlue = new XyDataSeries<double, float>();
            dataOrange = new XyDataSeries<double, float>();

            startCommand = new DelegateCommand(Start);
            stopCommand = new DelegateCommand(Stop);
            getDataCommand = new DelegateCommand(GetData);

            daqData = new DAQData();
            daqData.armDAQ();

            timer = new Timer(dt * 1000);
            timer.Elapsed += Timer_Elapsed;
        }

        public void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            daqData.ExtractData();
            dataBlue.Append(time, daqData.data1.Max());
            dataOrange.Append(time, daqData.data2.Max());
            time += dt;
        }

        private void Start()
        {
            timer.Start();
        }

        private void Stop()
        {
            timer.Stop();
        }

        private void GetData()
        {
            Timer_Elapsed(null, null);
        }
    }
}