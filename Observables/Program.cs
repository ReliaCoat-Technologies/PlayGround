using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Observables
{
    class Program
    {
        private static DAQ daq = new DAQ();
        private static int i = 0;

        static void Main(string[] args)
        {
            var isDeviceAvailable = daq.getDaqDevices();
            daq.setChannels();

            Console.WriteLine(daq.maxScanRate);
            Console.ReadKey();

            daq.arm();
            IObservable<long> source = Observable.Interval(TimeSpan.FromMilliseconds(100), Scheduler.Default);

            IDisposable subscription = source.Subscribe(WriteTime);

            Console.WriteLine("Press any key to unsubscribe");
            Console.ReadKey();
            subscription.Dispose();
        }

        public static void WriteTime(long time)
        {
            if (!daq.extractData()) return;
            i++;
            Console.WriteLine($"{DateTime.Now}, Data Collected {daq.DataBuffer.GetValue(7)}, Thread {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
