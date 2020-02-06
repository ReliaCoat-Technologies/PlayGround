using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SolutionCleaner
{
	class Program
	{
		static void Main(string[] args)
		{
			const string softwareDevFolder = @"D:\GitHub\";

			var files = Directory.EnumerateFiles(softwareDevFolder, "*.*", SearchOption.AllDirectories)
				.Where(x => x.Split('\\').All(y => y != "bin"))
				.Where(x => x.Split('\\').All(y => y != "obj"))
				.ToList();

			var filesCount = files.Count;

			Console.WriteLine("Initializing Github Database Zip");

			var sw = new Stopwatch();
			sw.Start();

			using (var fs = new FileStream(@"D:\GitHub.zip", FileMode.Create))
			{
				using (var zipFile = new ZipArchive(fs, ZipArchiveMode.Create, false))
				{
					for (var i = 0; i < filesCount; i++)
					{
						Console.WriteLine($"({i:D5}/{filesCount:D5}) Zipping {files[i]}");

						var zipName = files[i].Substring(softwareDevFolder.Length);
						zipFile.CreateEntryFromFile(files[i], zipName);
					}
				}
			}

			sw.Stop();

			Console.WriteLine($"Github directory ZIP complete. Time = {sw.Elapsed:g}");

			Console.ReadLine();
		}
	}
}
