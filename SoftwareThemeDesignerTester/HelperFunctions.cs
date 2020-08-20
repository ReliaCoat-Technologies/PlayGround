using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DevExpress.Mvvm.Native;

namespace SoftwareThemeDesignerTester
{
	public static class HelperFunctions
	{
		public static IEnumerable<Country> getCountryList()
		{
			var countryText = File.ReadAllLines("CountryList.csv");

			var headerLine = countryText.First().Split(',');
			var headerNumLines = headerLine.Length;

			var continentIndex = headerLine.IndexOf(x => x == "Continent_Name");
			var countryIndex = headerLine.IndexOf(x => x == "Country_Name");
			var continentCodeIndex = headerLine.IndexOf(x => x == "Continent_Code");
			var countryCodeIndex = headerLine.IndexOf(x => x == "Three_Letter_Country_Code");

			var isQuote = false;
			
			for (var i = 1; i < countryText.Length; i++)
			{
				var line = new List<string>();
				var lineSplit = countryText[i].Split(',');
				var sb = new StringBuilder();

				foreach (var item in lineSplit)
				{
					if (item.Contains("\""))
						isQuote = !isQuote;

					if (!isQuote && sb.Length == 0)
					{
						line.Add(item);
					}

					if (!isQuote && sb.Length > 0)
					{
						sb.Append(item);
						line.Add(sb.ToString());
						sb.Clear();
					}

					if (isQuote)
					{
						sb.Append($"{item},");
					}
				}

				yield return new Country(line[countryIndex], line[countryCodeIndex], line[continentIndex], line[continentCodeIndex]);
			}
		}
	}
}