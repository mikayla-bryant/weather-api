using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;


class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static string weatherConditions = "normal conditions";
    private static List<string> list = new List<string>();
    private static string location = "Millinocket";
    static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load();
        string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

        TwilioClient.Init(accountSid, authToken);
        var weatherData = await ProcessWeatherData();
        int currentTemp = (int)(weatherData.Main.Temp);
        int lowTemp = (int)(weatherData.Main.TempMin);
        int highTemp = (int)(weatherData.Main.TempMax);
     
       if(weatherData.Weather.Length == 1){
                weatherConditions = weatherData.Weather[0].Description;
        } else {
        foreach(Weather weather in weatherData.Weather)
        {
          list.Add(weather.Description);
        }
         weatherConditions = list.Count() > 1 ? string.Join(", ", list.Take(list.Count() - 1)) + " and " + list.Last() : list.FirstOrDefault();
        }
        var message = MessageResource.Create(
            body: $"Good morning! Here's your daily weather forecast for {location}. The current temperature is {currentTemp}°F. Today, you should expect a high of {highTemp}°F and a low of {lowTemp}°F. Weather Conditions: {weatherConditions}",
            from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("FROM")),
            to: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("TO"))
        );
    }
    private static async Task<WeatherData> ProcessWeatherData()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", ".NET Weather Text Notifications");

        var stringTask = client.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={location}&appid={Environment.GetEnvironmentVariable("OPEN_WEATHER_MAP_KEY")}&units=imperial");

        var weatherJson = await stringTask;
        var deserializedWeatherData = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherData>(weatherJson);
        return deserializedWeatherData;
    }
}