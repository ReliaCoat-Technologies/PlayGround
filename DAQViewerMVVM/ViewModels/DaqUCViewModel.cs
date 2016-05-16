using System;
using DevExpress.Mvvm;
using SciChart.Charting.Model.DataSeries;
using System.Timers;
using DAQViewer;
using System.Linq;
using System.Windows.Threading;

namespace DAQViewerMVVM.ViewModels
{
    public class DaqUCViewModel : ViewModelBase
    {
        XyDataSeries<double, float> _dataBlue;
        XyDataSeries<double, float> _dataOrange;

        public event Action Start;
        public event Action Stop;
        public event Action GetData;

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

        public DaqUCViewModel()
        {
            dataBlue = new XyDataSeries<double, float>();
            dataOrange = new XyDataSeries<double, float>();

            startCommand = new DelegateCommand(start);
            stopCommand = new DelegateCommand(stop);
            getDataCommand = new DelegateCommand(getData);
        }

        private void start()
        {
            Start?.Invoke();
        }

        private void stop()
        {
            Stop?.Invoke();
        }

        private void getData()
        {
            GetData?.Invoke();
        }
    }
}