using ACTIVEGIGELib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ActiveGigELibrary
{
    public class CameraCapture
    {
        #region Fields
        private IObservable<long> _timer;
        private IDisposable _subscriber;
        private List<byte[,]> _grandByteList;
        private int _width;
        private int _height;
        #endregion

        #region Properties
        public ActiveGige activeGige { get; set; }
        public byte[,] averageByteArray { get; set; }
        public double[] midPointLine { get; set; }
        public BitmapSource acquiredImage { get; set; }
        public bool isMeasuring { get; set; }
        #endregion

        #region Constructor
        public CameraCapture()
        {
            _grandByteList = new List<byte[,]>();

            activeGige = new ActiveGige
            {
                Camera = 0,
                Format = 0, // Always set to Mono8
                ExposureTime = 1000,
                Gain = 5.5f,
                Gamma = 1
            };

            activeGige.AcquisitionMode = "Continuous";
            activeGige.Acquire = true;
        }
        #endregion

        #region Methods
        private byte[,] acquireImageByteArray()
        {
            activeGige.Grab();
            activeGige.Flip = 2; // Horizontal flipping of images
            var ySize = activeGige.SizeY;
            var xSize = activeGige.SizeX;
            return activeGige.GetImageWindow(0, 0, Convert.ToInt16(xSize), Convert.ToInt16(ySize)) as byte[,];
        }

        public BitmapSource acquireImageBitmapSource()
        {
            var rawData = acquireImageByteArray();
            return convertToBitmapSource(rawData);
        }

        private BitmapSource convertToBitmapSource(byte[,] rawData)
        {
            _width = rawData.GetLength(0);
            _height = rawData.GetLength(1);

            if (isMeasuring) _grandByteList.Add(rawData);

            var bytes = unwind2DArray(rawData);
            return BitmapSource.Create(_width, _height, 100, 100, PixelFormats.Gray8, null, bytes, _width);
        }

        private byte[] unwind2DArray(byte[,] rawData)
        {
            // Introduce thresholding operation here
            var byteList = new List<byte>();
            for (var j = 0; j < _height; j++)
                for (var i = 0; i < _width; i++)
                    byteList.Add(rawData[i, j]);

            return byteList.ToArray();
        }

        public BitmapSource getAverageImage()
        {
            averageByteArray = new byte[_width, _height];
            midPointLine = new double[_width];

            Parallel.For(0, _height, j =>
            {
                Parallel.For(0, _width, i =>
                {
                    averageByteArray[i, j] = getPixelAverageBytes(i, j);
                    if (j == _height / 2) midPointLine[i] = getPixelAverageBytes(i, j);
                });
            });

            var averageByteArrayList = unwind2DArray(averageByteArray);
            return BitmapSource.Create(_width, _height, 100, 100, PixelFormats.Gray8, null, averageByteArrayList, _width);
        }

        private byte getPixelAverageBytes(int xValue, int yValue)
        {
            var query = getPixelAverageDouble(xValue, yValue);
            return Convert.ToByte(query);
        }

        private double getPixelAverageDouble(int xValue, int yValue)
        {
            return _grandByteList.Select(x => Convert.ToInt32(x[xValue, yValue])).Average();
        }
        #endregion
    }
}
