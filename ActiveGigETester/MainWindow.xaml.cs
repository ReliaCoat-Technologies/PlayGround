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
            // Automatically detects available cameras.
            activeGige = new ActiveGige
            {
                Camera = 0, // Connects to the first available camera
                Acquire = true, // Allows frame acquisition
                Format = 0, // For PixelLink - Mono8 = 0, Mono16 = 1
            };

            // Not essential to operating -- Simply shows characteristics of camera
            var ipList = activeGige.GetCameraIPList();
            var maxFrameRate = activeGige.GetAcquisitionFrameRateMax();
            var minFrameRate = activeGige.GetAcquisitionFrameRateMin();
            var maxExposure = activeGige.GetExposureTimeMax();
            var minExposure = activeGige.GetExposureTimeMin();
            var maxGain = activeGige.GetGainMax();
            var minGain = activeGige.GetGainMin();
            var maxGamma = activeGige.GetGammaMax();
            var minGamma = activeGige.GetGammaMin();

            // Properties available in Matlab version of PlumeOPT now reflected in C#
            activeGige.ExposureTime = 1000;
            activeGige.Gain = 5.5f;
            activeGige.Gamma = 1;

            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            timer = Observable.Interval(TimeSpan.FromSeconds(0.1)).SubscribeOn(Scheduler.Default).ObserveOnDispatcher();
            subscriber = timer.Subscribe(acquireImage);
        }

        private void acquireImage(long i)
        {
            // Acquires a single frame
            activeGige.Grab();

            // This method leads to a dynamic type
            // Mono8 will get you a 2D byte array, Mono16 will get you a 2D short array
            var rawData = activeGige.GetRawData() as byte[,]; 

            imageHolder.Source = convertToBitmapSource(rawData);
            TimeBlock.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            SampleBlock.Text = i.ToString();
            if(i >= 100) subscriber.Dispose();

            // Image sequences are possble, API shows how it's done
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

        private void Stop(object sender, RoutedEventArgs e)
        {
            subscriber.Dispose();
        }
    }
}
