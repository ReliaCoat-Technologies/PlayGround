using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace ImageCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();

            var list = new List<int>();

            for (var i = 0; i < 320000; i++)
            {
                // list.Add(0);
                list.Add(random.Next(256 * 10000 - 1));
            }

            var sw = new Stopwatch();
            Console.WriteLine($"Original List Size = {getObjectSize(list):F2} MB");

            sw.Start();
            var jsonString = JsonConvert.SerializeObject(list);
            sw.Stop();
            Console.WriteLine($"JSON Serialization Time = {sw.Elapsed.TotalMilliseconds:F2} ms");
            var jsonMemory = getObjectSize(jsonString);
            Console.WriteLine($"JSON Array Size = {jsonMemory:F2} MB");

            sw.Restart();
            var gZipCompressedStringObj = new CompressedIntPng(list);
            sw.Stop();
            Console.WriteLine($"Array Compression Time = {sw.Elapsed.TotalMilliseconds:F2} ms");

            sw.Restart();
            var compressedArray = new CompressedIntArray(list, IntCompressionUnicodeFormat.bit8);
            var arrayString = compressedArray.compressedArray;
            sw.Stop();
            Console.WriteLine($"Array Compression Time = {sw.Elapsed.TotalMilliseconds:F2} ms");
            Console.WriteLine($"Number of Byte Digits = {compressedArray.numByteDigits} digits");
            var compressedMemory = getObjectSize(arrayString);
            Console.WriteLine($"Compressed Array Size = {compressedMemory:F2} MB");

            sw.Restart();
            var compressedArray16 = new CompressedIntArray(list, IntCompressionUnicodeFormat.bit16);
            var arrayString16 = compressedArray.compressedArray;
            sw.Stop();
            Console.WriteLine($"Array Compression Time (16-bit) = {sw.Elapsed.TotalMilliseconds:F2} ms");
            Console.WriteLine($"Number of Byte Digits (16-bit) = {compressedArray16.numByteDigits} digits");
            var compressedMemory16 = getObjectSize(arrayString16);
            Console.WriteLine($"Compressed Array Size (16-bit) = {compressedMemory16:F2} MB");

            sw.Restart();
            var listDecompressed = compressedArray.decompressIntArray(arrayString);
            sw.Stop();
            Console.WriteLine($"Array De-Compression Time = {sw.Elapsed.TotalMilliseconds:F2} ms");

            var memorySaved = (1 - compressedMemory / jsonMemory) * 100;
            var memorySaved16 = (1 - compressedMemory16 / jsonMemory) * 100;

            // Console.WriteLine(arrayString);
            Console.WriteLine(arrayString.Length);
            Console.WriteLine($"Memory Saved - {jsonMemory - compressedMemory:F2} MB ({memorySaved:F1} %)");
            Console.WriteLine($"Memory Saved (16-bit) - {jsonMemory - compressedMemory16:F2} MB ({memorySaved16:F1} %)");
            Console.WriteLine(list.SequenceEqual(listDecompressed));

            Console.ReadLine();
        }

        static double getObjectSize(object o)
        {
            using (var s = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                var numBytes = s.Length;
                return (double)numBytes * Math.Pow(10, -6);
            }
        }
    }
}
