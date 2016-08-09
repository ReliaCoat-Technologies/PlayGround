using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAQCOMLib;

namespace Observables
{
    public class DAQ
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

        public int numberOfScans { get; set; }
        public double maxScanRate => pAcq.Config.MaxScanRate;

        public Array DataBuffer => dataBuffer;
        private Array dataBuffer;

        /// <summary>
        /// Starts up DAQ
        /// </summary>
        /// <param name="scanCount">Number of Scans Total per Cycle</param>
        /// <param name="scanRate">Number of Scans per Second per Cycle</param>
        public DAQ()
        {
            try { pSys = new DaqSystem(); }
            catch { throw new Exception("DAQ Driver Unavailable"); }
            pAcq = pSys.Add();
            pConfig = pAcq.Config;
            pScanList = pConfig.ScanList;
            pSysDevices = pAcq.AvailableDevices;
            pAcq.DataStore.AutoSizeBuffers = true;
            pAcq.DataStore.IgnoreDataStoreOverruns = true;
            pAcq.DataStore.PeekNewest = true;
        }

        public bool getDaqDevices()
        {
            var count = pSysDevices.Count;
            if (count > 0)
            {
                // Collects the first (and possibly only) DAQ device available
                pDev = pSysDevices.CreateFromIndex(count);

                // Prompts the DAQ (confirms connection)
                pDaq = (Daq3xxx)pDev;
                pDaq.OverSampleMultiplier = OverSampleMultiplier.osmX256; // Oversampler
                pDaq.LineCycleRejection = LineCycleRejection.lcrOff; //Line Cycle Rejection
                if (!pDev.IsOpen) pDev.Open();
                return true;
            }
            return false;
        }

        public void setChannels(int scanCount = 1, int scanRate = 500)
        {
            pAnalogInputs = pDev.AnalogInputs;
            pAvailableChannels = pDev.AvailableBaseChannels;

            pConfig.ScanCount = scanCount;
            pConfig.ScanRate = scanRate;

            pDaq.AnalogOutputs.RemoveAll();

            // TC1
            BaseChannel baseChannel = pAvailableChannels.get_ItemByName("DaqChannel3");
            pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
            pChannel = (pDaq3000DirectAIChannel)pAnalogInput.Channels[1];
            pChannel.SelectedRange = pChannel.Ranges.ItemByType[RangeType.rtTypeK];
            pChannel.EngrUnits = UnitType.utDegreesC;
            pChannel.DifferentialMode = true;
            pChannel.AddToScanList();

            // TC2
            baseChannel = pAvailableChannels.get_ItemByName("DaqChannel4");
            pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
            pChannel = (pDaq3000DirectAIChannel)pAnalogInput.Channels[1];
            pChannel.SelectedRange = pChannel.Ranges.ItemByType[RangeType.rtTypeK];
            pChannel.EngrUnits = UnitType.utDegreesC;
            pChannel.DifferentialMode = true;
            pChannel.AddToScanList();

            // TC4 (user?)
            baseChannel = pAvailableChannels.get_ItemByName("DaqChannel6");
            pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
            pChannel = (pDaq3000DirectAIChannel)pAnalogInput.Channels[1];
            pChannel.SelectedRange = pChannel.Ranges.ItemByType[RangeType.rtTypeK];
            pChannel.EngrUnits = UnitType.utDegreesC;
            pChannel.DifferentialMode = true;
            pChannel.AddToScanList();

            // Laser 1
            baseChannel = pAvailableChannels.get_ItemByName("DaqChannel2");
            pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
            pAnalogInput.Channels[1].AddToScanList();

            // Laser 2
            baseChannel = pAvailableChannels.get_ItemByName("DaqChannel1");
            pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
            pAnalogInput.Channels[1].AddToScanList();

            // Laser 3
            baseChannel = pAvailableChannels.get_ItemByName("DaqChannel0");
            pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
            pAnalogInput.Channels[1].AddToScanList();

            // Pyrometer 1
            baseChannel = pAvailableChannels.get_ItemByName("DaqChannel7");
            pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
            pAnalogInput.Channels[1].AddToScanList();

            // Pyrometer 2
            baseChannel = pAvailableChannels.get_ItemByName("DaqChannel15");
            pAnalogInput = pAnalogInputs.Add(AnalogInputType.aitDirect, baseChannel.BaseChannel, 0);
            pAnalogInput.Channels[1].AddToScanList();

            // Number of Items on the Scan List
            numberOfScans = pScanList.Count;

            // Acquisition starts on arm, measured on "start", stopped manually (on "stop)
            pAcq.Starts.ItemByType[StartType.sttImmediate].UseAsAcqStart();
            pAcq.Stops.ItemByType[StopType.sptManual].UseAsAcqStop();

            // Creating the dataBuffer array
            dataBuffer = new float[pConfig.ScanCount * numberOfScans];
        }

        public void arm()
        {
            // Arming the DAQ (which starts acquisition)
            if (!pAcq.Active)
                pAcq.Arm();
        }

        public void disarm()
        {
            if (pAcq.Active)
                pAcq.Disarm();
        }

        public bool extractData()
        {
            // var returnedScans = pAcq.DataStore.PeekData(ref dataBuffer);
            if (pAcq.DataStore.AvailableScans == 0) return false;
            pAcq.DataStore.FetchData(ref dataBuffer);
            return true;
        }

        public int getScanCount()
        {
            return pConfig.ScanCount;
        }
    }
}
