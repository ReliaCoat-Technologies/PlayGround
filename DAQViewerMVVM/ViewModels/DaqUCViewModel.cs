using System;
using DevExpress.Mvvm;
using SciChart.Charting.Model.DataSeries;
using System.Timers;
using DAQViewer;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAQViewerMVVM.ViewModels
{
    public class DaqUCViewModel : ViewModelBase
    {
        public event Action Start;
        public event Action Stop;
        public event Action GetData;

        /*
        public DAQData daqData;
        public Timer timer;
        public const double dt = 0.10;
        private double time;
        */

        public DelegateCommand startCommand { get; set; }
        public DelegateCommand stopCommand { get; set; }
        public DelegateCommand getDataCommand { get; set; }

        public XyDataSeries<double, float> dataBlue { get; set; }
        public XyDataSeries<double, float> dataOrange { get; set; }
        

        public DaqUCViewModel()
        {
            dataBlue = new XyDataSeries<double, float>();
            dataOrange = new XyDataSeries<double, float>();

            /*
            daqData = new DAQData();
            daqData.armDAQ();

            timer = new Timer(dt * 1000);
            timer.Elapsed += Timer_Elapsed;
            */

            startCommand = new DelegateCommand(start);
            stopCommand = new DelegateCommand(stop);
            getDataCommand = new DelegateCommand(getData);
        }

        /*
        public void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Array array;
            array = daqData.ExtractData();

            foreach(var item in array)
            dataBlue.Append(time, daqData.data1.Max());
            dataOrange.Append(time, daqData.data2.Max());
            time += dt;
        }
        */

        private void start()
        {
            Start?.Invoke();
            //timer.Start();
        }

        private void stop()
        {
            Stop?.Invoke();
            //timer.Stop();
        }

        private void getData()
        {
            GetData?.Invoke();
            // Timer_Elapsed(null, null);
        }
    }
}