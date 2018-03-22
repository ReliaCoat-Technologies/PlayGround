using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCompression
{
    public enum IntCompressionUnicodeFormat
    {
        bit8,
        bit16
    }

    public class CompressedIntArray
    {
        private IEnumerable<int> _originalArray;
        private int _originalBytePower;

        public int numByteDigits { get; set; }
        public string compressedArray { get; set; }
        public IntCompressionUnicodeFormat compressionFormat { get; set; }

        public CompressedIntArray(IEnumerable<int> intArray, IntCompressionUnicodeFormat compressionFormat = IntCompressionUnicodeFormat.bit8)
        {
            setCompressionFormat(compressionFormat);

            var maxValue = intArray.Max();

            long bytePower = _originalBytePower;
            numByteDigits = 1;

            while (maxValue > bytePower)
            {
                bytePower *= _originalBytePower;
                numByteDigits++;
            }

            compressedArray = compressIntArray(intArray);
        }

        public CompressedIntArray(int numByteDigits, IntCompressionUnicodeFormat compressionFormat = IntCompressionUnicodeFormat.bit8)
        {
            setCompressionFormat(compressionFormat);

            if (numByteDigits <= 0)
                throw new ArgumentException("Number of Bits must be greater than 0");

            this.numByteDigits = numByteDigits;
        }

        public string compressIntArray(IEnumerable<int> intArray)
        {
            _originalArray = intArray;

            var sb = new StringBuilder();

            foreach (var item in intArray)
                sb.Append(intToUtf(item));

            compressedArray = sb.ToString();

            return compressedArray;
        }

        private void setCompressionFormat(IntCompressionUnicodeFormat compressionFormat)
        {
            this.compressionFormat = compressionFormat;

            switch (compressionFormat)
            {
                case IntCompressionUnicodeFormat.bit8:
                    _originalBytePower = 256;
                    break;
                case IntCompressionUnicodeFormat.bit16:
                    _originalBytePower = 256 * 256;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compressionFormat), compressionFormat, null);
            }
        }

        private string intToUtf(int number)
        {
            var charArray = new char[numByteDigits];

            for (var i = 0; i < numByteDigits; i++)
            {
                var power = (int)Math.Pow(_originalBytePower, numByteDigits - 1 - i);
                var byteValue = number / power;
                number -= power * byteValue;
                charArray[i] = Convert.ToChar(byteValue);
            }

            var finalString = new string(charArray);

            return finalString;
        }

        public IEnumerable<int> decompressIntArray(string inputString)
        {
            var numThings = inputString.Length / numByteDigits;
            var intArray = new int[numThings];

            var chars = inputString.ToCharArray();

            Parallel.For(0, numThings, i =>
            {
                var selectedChars = new char[numByteDigits];

                for (var j = 0; j < numByteDigits; j++)
                    selectedChars[j] = chars[numByteDigits * i + j];

                intArray[i] = utfToInt(selectedChars);
            });

            return intArray;
        }

        private int utfToInt(char[] inputString)
        {
            var numDig = numByteDigits;
            var number = 0;

            foreach (var item in inputString)
            {
                var byteNumber = Convert.ToInt32(item);
                var power = (int)Math.Pow(_originalBytePower, numDig - 1);
                number += power * byteNumber;
                numDig -= 1;
            }

            return number;
        }
    }
}