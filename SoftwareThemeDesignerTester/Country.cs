namespace SoftwareThemeDesignerTester
{
	public class Country
	{
		public string countryCode { get; }
		public string countryName { get; }
		public string continentName { get; }
		public string continentCode { get; }

		public Country(string countryName, string countryCode, string continentName, string continentCode)
		{
			this.countryCode = countryCode;
			this.countryName = countryName.Trim('\"');
			this.continentCode = continentCode;
			this.continentName = continentName;
		}
	}
}