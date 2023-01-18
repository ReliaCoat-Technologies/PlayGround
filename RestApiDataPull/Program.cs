// See https://aka.ms/new-console-template for more information

using RestSharp;

Console.WriteLine("Hello, World!");

const int ZIP = 11716;
const string language = "en";

var client = new RestClient("https://weatherapi-com.p.rapidapi.com/history.json");

for (var i = 0; i < 10; i++)
{
    var day = DateTime.Today.AddDays(-(1 + i));
    
    var dayString = day.ToString("yyyy-MM-dd");

    var request = new RestRequest($"?q={ZIP}&dt={dayString}&lang={language}");
    request.AddHeaders(new Dictionary<string, string>
    {
        { "X-RapidAPI-Key", "7304d83c16mshc02f440bfee9512p195ad4jsnbb975d707f6f" },
        { "X-RapidAPI-Host", "weatherapi-com.p.rapidapi.com" }
    });

    var response = await client.GetAsync(request);

    var content = response.Content;
}