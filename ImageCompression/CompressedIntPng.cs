using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ImageCompression
{
    public class CompressedIntPng
    {
        public CompressedIntPng(IEnumerable<int> intArray)
        {
            var a = compress(intArray);
        }

        private static BitmapImage compress(IEnumerable<int> intArray)
        {
            var ints = intArray.ToArray();
            var bytes = new byte[ints.Length * sizeof(int)];
            Buffer.BlockCopy(ints, 0, bytes, 0, bytes.Length);

            using (var ms = new MemoryStream(bytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();

                return image;
            }
        }
    }
}
