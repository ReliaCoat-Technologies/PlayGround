using DAQCOMLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DAQTester
{
    internal class DAQSomething
    {
        DaqSystem pSys;
        Acq pAcq;
        AvailableDevices pSysDevices;
        Devices pDevs;
        Device pDev;

        Config pConfig;
        ScanList pScanList;
        IAnalogInput pAnalogInput;
        IAnalogInput pAnalogInput2;
        IAnalogInputs pAnalogInputs;
        AvailableBaseChannels pAvailableChannels;
        BaseChannel pBaseChannel;
        SetPoint pSetPoint;

        Daq3xxx pDaq;

        bool DEV;
        SetPoint SP1;
        Port pPortC;
        DigitalIO pDIO;
        DigitalIO pSpStat;
        Port pPort;

        public DAQSomething()
        {
            pSys = new DaqSystem();
            pAcq = pSys.Add();
            pConfig = pAcq.Config;
            pScanList = pConfig.ScanList;
            pSysDevices = pAcq.AvailableDevices;
            pAcq.DataStore.AutoSizeBuffers = false;
            pAcq.DataStore.BufferSizeInScans = 100000;
            pAcq.DataStore.IgnoreDataStoreOverruns = true;

            var count = pSysDevices.Count;
            Console.WriteLine($"{count.ToString()} devices detected:");
            for (int i = 1; i <= count; i++)
                Console.WriteLine(pSysDevices[i]);
        }

        public void startMeasurement()
        {
            var count = pSysDevices.Count;
            if (count > 0)
            {
                var pDev = pSysDevices.CreateFromIndex(1);
                Console.WriteLine(pDev.DeviceType);

                pDaq = (Daq3xxx)pDev;
                pDev.Open();

                Console.WriteLine(pDaq.Name.ToString());
                pAnalogInputs = pDev.AnalogInputs;

                // pConfig.ScanCount = 100;
                // pConfig.ScanRate = 1000;

                pAvailableChannels = pDev.AvailableBaseChannels;

                foreach (BaseChannel bc in pAvailableChannels)
                    Console.WriteLine($"BaseChannel: Name={bc.Name}, Index={bc.Index}, BaseChannel={bc.BaseChannel.ToString()}");

                pDev.AnalogOutputs.RemoveAll();

                BaseChannel baseChannel = pAvailableChannels.get_ItemByName("DaqChannel2");
                pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, (DeviceModulePosition)0);
                pAnalogInput.Channels[1].AddToScanList();

                baseChannel = pAvailableChannels.get_ItemByName("DaqChannel10");
                pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, (DeviceModulePosition)0);
                pAnalogInput.Channels[1].AddToScanList();

                pAcq.Starts.ItemByType[StartType.sttImmediate].UseAsAcqStart();
                pAcq.Stops.ItemByType[StopType.sptManual].UseAsAcqStop();

                pAcq.Arm();

                Console.WriteLine("System Started!");

                Console.WriteLine(pScanList.Count.ToString());
                Console.WriteLine(pConfig.ScanCount);
            }
        }

        public void ExtractData()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            Array _dataBuffer = new float[200];
            int returnedScans = pAcq.DataStore.FetchData(ref _dataBuffer, 100);

            var arrayAsList1 = new List<float>();
            var arrayAsList2 = new List<float>();

            bool switcher = true;
            foreach (var item in _dataBuffer)
            {
                if (switcher == true) arrayAsList1.Add((float)item);
                if (switcher == false) arrayAsList2.Add((float)item);
                switcher = !switcher;
            }

            Console.WriteLine($"----------- {stopwatch.Elapsed} ms,  {returnedScans} ------------");
            Console.WriteLine($"{arrayAsList1.Max()}, {arrayAsList1.Min()}, {RootMeanSquare(arrayAsList1)}");
            Console.WriteLine($"{arrayAsList2.Max()}, {arrayAsList2.Min()}, {RootMeanSquare(arrayAsList2)}");

            Thread.Sleep(250);
        }

        private static float RootMeanSquare(IList<float> values)
        {
            double s = 0;

            foreach (var item in values)
                s += item * item;

            return (float)Math.Sqrt(s / values.Count());
        }
    }
}
