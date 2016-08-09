using System.Windows;
using System.Windows.Media.Imaging;
using ACTIVEGIGELib;
using System.Windows.Media;
using System.Collections.Generic;
using System.Reactive.Linq;
using System;
using System.Globalization;
using System.Reactive.Concurrency;

namespace ActiveGigETester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ActiveGige activeGige;
        private IObservable<long> timer;
        private IDisposable subscriber;

        public MainWindow()
        {
            activeGige = new ActiveGige
            {
                Camera = 0,
                Acquire = true,
                Format = 0,
            };

            var ipList = activeGige.GetCameraIPList();
            var maxFrameRate = activeGige.GetAcquisitionFrameRateMax();
            var minFrameRate = activeGige.GetAcquisitionFrameRateMin();
            var maxExposure = activeGige.GetExposureTimeMax();
            var minExposure = activeGige.GetExposureTimeMin();
            var maxGain = activeGige.GetGainMax();
            var minGain = activeGige.GetGainMin();
            var maxGamma = activeGige.GetGammaMax();
            var minGamma = activeGige.GetGammaMin();

            activeGige.ExposureTime = 100;
            activeGige.Gain = 5.5f;
            activeGige.Gamma = 1;

            InitializeComponent();
        }

        private void Acquire(object sender, RoutedEventArgs e)
        {
            timer = Observable.Interval(TimeSpan.FromSeconds(0.1)).SubscribeOn(Scheduler.Default).ObserveOnDispatcher();
            subscriber = timer.Subscribe(acquireImage);
        }

        private void acquireImage(long i)
        {
            activeGige.Grab();
            var rawData = activeGige.GetRawData() as byte[,];
            imageHolder.Source = convertToBitmapSource(rawData);
            TimeBlock.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            SampleBlock.Text = i.ToString();

            if(i >= 100) subscriber.Dispose();
        }

        private BitmapSource convertToBitmapSource(byte[,] rawData)
        {
            var width = rawData.GetLength(0);
            var height = rawData.GetLength(1);

            var byteList = new List<byte>();
            for(var j = 0; j < height; j++)
                for (var i = 0; i < width; i++)
                    byteList.Add(rawData[i,j]);

            var bytes = byteList.ToArray();

            return BitmapSource.Create(width, height, 100, 100, PixelFormats.Gray8, null, bytes, width);
        }

        private void StopSimpleButton_OnClick(object sender, RoutedEventArgs e)
        {
            subscriber.Dispose();
        }
    }
}
