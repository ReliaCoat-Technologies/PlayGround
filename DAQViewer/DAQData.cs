using DAQCOMLib;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace DAQViewer
{
    public class DAQData
    {
        DaqSystem pSys;
        Acq pAcq;
        AvailableDevices pSysDevices;
        IDevice pDev;

        Config pConfig;
        ScanList pScanList;
        IAnalogInput pAnalogInput;
        IAnalogInputs pAnalogInputs;
        AvailableBaseChannels pAvailableChannels;
        pDaq3000DirectAIChannel pChannel;
        Daq3xxx pDaq;

        Array _dataBuffer;

        public List<float> data1;
        public List<float> data2;

        public DAQData()
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

        public void armDAQ()
        {
            var count = pSysDevices.Count;
            if (count > 0)
            {
                pDev = pSysDevices.CreateFromIndex(1);
                Console.WriteLine(pDev.DeviceType);

                pDaq = (Daq3xxx)pDev;
                pDaq.OverSampleMultiplier = OverSampleMultiplier.osmX1024;
                pDaq.LineCycleRejection = LineCycleRejection.lcr60HzTC;
                pDaq.Open();

                Console.WriteLine(pDaq.Name.ToString());
                pAnalogInputs = pDev.AnalogInputs;

                pAvailableChannels = pDev.AvailableBaseChannels;

                pConfig.ScanCount = 1;
                pConfig.ScanRate = pConfig.MaxScanRate;

                Console.WriteLine(pConfig.ScanCount.ToString());
                Console.WriteLine(pConfig.ScanRate.ToString());
                
                foreach (BaseChannel bc in pAvailableChannels)
                    Console.WriteLine($"BaseChannel: Name={bc.Name}, Index={bc.Index}, BaseChannel={bc.BaseChannel.ToString()}");

                pDev.AnalogOutputs.RemoveAll();

                BaseChannel baseChannel = pAvailableChannels.get_ItemByName("DaqChannel3");
                pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
                pChannel = (pDaq3000DirectAIChannel)pAnalogInput.Channels[1];
                pChannel.SelectedRange = pChannel.Ranges.ItemByType[RangeType.rtTypeK];
                pChannel.EngrUnits = UnitType.utDegreesC;
                pChannel.DifferentialMode = true;
                pChannel.AddToScanList();
                
                // For non-TC measurements
                baseChannel = pAvailableChannels.get_ItemByName("DaqChannel1");
                pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
                pAnalogInput.Channels[1].AddToScanList();

                pAcq.Starts.ItemByType[StartType.sttImmediate].UseAsAcqStart();
                pAcq.Stops.ItemByType[StopType.sptManual].UseAsAcqStop();
                
                _dataBuffer = new float[2]; // Expensive operation, do NOT put in ExtractData

                pAcq.DataStore.BufferMode = DataBufferMode.dbmDataStore;
                pAcq.Arm();
            }
        }

        public void disarmDaq()
        {
            pAcq.Disarm();
        }

        public Array ExtractData()
        {
            int returnedScans = pAcq.DataStore.FetchData(ref _dataBuffer, 1);

            data1 = new List<float>();
            data2 = new List<float>();

            bool switcher = false;
            foreach (var item in _dataBuffer)
            {
                if (switcher == false) data1.Add((float)item);
                else data2.Add((float)item);
                switcher = !switcher;
            }

            pAcq.DataStore.FlushData();

            return _dataBuffer;
        }
    }
}
