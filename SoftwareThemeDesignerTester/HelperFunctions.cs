using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SoftwareThemeDesignerTester
{
	public static class HelperFunctions
	{
		public static async Task<IList<Country>> getCountriesAsync()
		{
			var request = WebRequest.Create("https://restcountries.eu/rest/v2/all");
			request.Method = "GET";

			var response = await request.GetResponseAsync();

			var responseJson = string.Empty;

			using (var stream = response.GetResponseStream())
				using (var sr = new StreamReader(stream))
					responseJson = await sr.ReadToEndAsync();

			var result = JsonConvert.DeserializeObject<IList<Country>>(responseJson, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore,
			});

			return result;
		}
	}
}