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
        public DAQData daqData;
        public Timer timer;
        public const double dt = 0.10;
        private double time;

        public DaqUCViewModel daqUCViewModel { get; set; }

        public MainWindowViewModel()
        {
            daqUCViewModel = new DaqUCViewModel();
            daqUCViewModel.Start += Start;
            daqUCViewModel.Stop += Stop;
            daqUCViewModel.GetData += GetData;

            daqData = new DAQData();
            daqData.armDAQ();

            timer = new Timer(dt * 1000);
            timer.Elapsed += Timer_Elapsed;
        }

        public void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var array = daqData.ExtractData();
            daqUCViewModel.dataBlue.Append(time, daqData.data1.Max());
            daqUCViewModel.dataOrange.Append(time, daqData.data2.Max());
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