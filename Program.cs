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
        var message = MessageResource.Create(
            body: $"Good morning! Here's your daily weather forecast. The current temperature is {currentTemp}°F. With a high of {highTemp} and a low of {lowTemp}",
            from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("FROM")),
            to: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("TO"))
        );
    }
    private static async Task<WeatherData> ProcessWeatherData()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

        var stringTask = client.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q=jacksonville&appid={Environment.GetEnvironmentVariable("OPEN_WEATHER_MAP_KEY")}&units=imperial");

        var weatherJson = await stringTask;
        var deserializedWeatherData = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherData>(weatherJson);
        return deserializedWeatherData;
    }
}