using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliaCoat.Common;

namespace XrdFileConverter
{
	class Program
	{
		static void Main(string[] args)
		{
			var files = Directory.GetFiles(@"D:\RCT3037\2021-08-17 - Substrate Analysis\Raw XRD Data");

			var xrdFiles = files
				.Where(x => Path.GetExtension(x) == ".ras")
				.ToList();

			foreach (var xrdFile in xrdFiles)
			{
				convertXrdRasFile(xrdFile);
			}
		}

		public static void convertXrdRasFile(string fileName)
		{
			var lines = File.ReadAllLines(fileName);

			var data = lines
				.Select(x => x.Split(' '))
				.Where(x => x.Length == 3)
				.Select(getRowData)
				.Where(x => x != null)
				.ToList();

			var csvSave = FileExtensions.appendToFileName(fileName, string.Empty, ".csv");

			using (var sw = new StreamWriter(csvSave))
			{
				sw.WriteLine("Angle (°), Intensity");

				foreach (var line in data)
				{
					sw.WriteLine($"{line[0]},{line[1]}");
				}
			}

			Console.WriteLine($"Converted {fileName}");
		}

		public static double[] getRowData(string[] rawRowData)
		{
			if (rawRowData.Length != 3)
			{
				return null;
			}

			var parsedData = rawRowData.Select(x =>
				{
					double result;
					
					return double.TryParse(x, out result) 
						? result 
						: double.NaN;
				})
				.ToArray();

			return parsedData.Any(double.IsNaN)
				? null
				: parsedData;
		}
	}
}
