using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SoftwareThemeDesignerTester
{
	public static class HelperFunctions
	{
		public static async Task<IList<Country>> getCountriesAsync()
		{
			var countryList = await getCountries();

			return countryList;
		}

		private static async Task<IList<Country>> getCountries()
		{
			var request = WebRequest.Create("https://parseapi.back4app.com/classes/Country?count=1&include=continent");
			request.Headers.Add("X-Parse-Application-Id", "mxsebv4KoWIGkRntXwyzg6c6DhKWQuit8Ry9sHja");
			request.Headers.Add("X-Parse-Master-Key", "TpO0j3lG2PmEVMXlKYQACoOXKQrL3lwM0HwR9dbH");
			request.Method = "GET";

			var response = await request.GetResponseAsync();

			var responseJson = string.Empty;

			using (var stream = response.GetResponseStream())
				using (var sr = new StreamReader(stream))
					responseJson = await sr.ReadToEndAsync();

			var results = JsonConvert.DeserializeObject<JObject>(responseJson);

			var countriesJson = results["results"] as JArray;

			return countriesJson?
				.OfType<JObject>()
				.Select(x => new Country(x))
				.ToList();
		}
	}
}