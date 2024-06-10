using System;
using System.Net.Http;
using System.Data.SqlTypes;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestNow
{
    class Program
    {
        static ITelegramBotClient botClient;

        static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient("YOUR_BOT_API_KEY_HERE");
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hi, my username is {me.Id} my name is {me.FirstName}.");
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            botClient.StopReceiving();
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");
                string weatherInfo = await GetWeatherInfo(e.Message.Text);
                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: weatherInfo);
            }
        }

        static async Task<string> GetWeatherInfo(string message)
        {
            string apiKey = "YOUR_OPENWEATHERMAP_API_KEY_HERE";
            string apiURL = $"https://api.openweathermap.org/data/2.5/weather?lat=43.0033&lon=41.0153&appid={apiKey}&units=metric";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiURL);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<WeatherResponse>(responseBody);
                    string weatherDescription = weatherData.weather[0].description;
                    string temp = weatherData.main.temp.ToString("0.0");

                    return $"Погода: {weatherDescription}, Температура: {temp}°C";
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message : {0}", e.Message);
                    return "Не удалось получить данные о погоде.";
                }
            }
        }

        public class Weather
        {
            public string description { get; set; }
        }

        public class Main1
        {
            public float temp { get; set; }
        }

        public class WeatherResponse
        {
            public Weather[] weather { get; set; }
            public Main1 main { get; set; }
        }
    }
}