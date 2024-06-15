using System;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;

namespace TestNow
{
    public class Weather
    {
        public string Description { get; set; }
    }

    public class TempInfo
    {
        public float Temp { get; set; }
    }

    public class WeatherResponse
    {
        public Weather[] Weather { get; set; }
        public TempInfo Main { get; set; }
    }

    class Program
    {
        const string TEXT_1 = "Сухум";
        const string TEXT_2 = "Гудаута";
        const string TEXT_3 = "Лидзава";
        const string TEXT_4 = "Новый Афон";
        const string TEXT_5 = "Очамчира";
        static ITelegramBotClient botClient;

        static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient("7467417422:AAEAAMYecsSPSBnNHQw_dxUkrAOm1Xux7oc");
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

                if (e.Message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat.Id,
                        text: "Выберите регион для получения погоды:",
                        replyMarkup: GetButtons()
                    );
                }
                else
                {
                    string weatherInfo = await GetWeatherInfo(e.Message.Text);
                    await botClient.SendTextMessageAsync(chatId: e.Message.Chat.Id, text: weatherInfo);
                }
            }
        }

        public static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>> {
                    new List<KeyboardButton>{new KeyboardButton { Text = TEXT_1 }, new KeyboardButton { Text = TEXT_2 } },
                    new List<KeyboardButton>{new KeyboardButton { Text = TEXT_3 }, new KeyboardButton { Text = TEXT_4 } },
                    new List<KeyboardButton>{new KeyboardButton { Text = TEXT_5 }}
                },
                ResizeKeyboard = true
            };
        }

        static async Task<string> GetWeatherInfo(string location)
        {
            string apiKey = "a3d6d40ea2d819407c5923d19ac5a418";
            string lang = "ru";
            string url = location switch
            {
                TEXT_1 => $"https://api.openweathermap.org/data/2.5/weather?lat=43.0033&lon=41.0153&appid={apiKey}&units=metric&lang={lang}",
                TEXT_2 => $"https://api.openweathermap.org/data/2.5/weather?lat=43.1055&lon=40.6207&appid={apiKey}&units=metric&lang={lang}",
                TEXT_3 => $"https://api.openweathermap.org/data/2.5/weather?lat=43.1781&lon=40.3675&appid={apiKey}&units=metric&lang={lang}",
                TEXT_4 => $"https://api.openweathermap.org/data/2.5/weather?lat=43.0806&lon=40.8383&appid={apiKey}&units=metric&lang={lang}",
                TEXT_5 => $"https://api.openweathermap.org/data/2.5/weather?lat=42.7123&lon=41.4686&appid={apiKey}&units=metric&lang={lang}",
                _ => null
            };

            if (url == null)
            {
                return "Неизвестный регион. Пожалуйста, выберите регион с помощью кнопок.";
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<WeatherResponse>(responseBody);
                    string weatherDescription = weatherData.Weather[0].Description;
                    string temp = weatherData.Main.Temp.ToString("0.0");

                    return $"Погода в городе {location}: {weatherDescription}, Температура: {temp}°C";
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message : {0}", e.Message);
                    return "Не удалось получить данные о погоде.";
                }
            }
        }
    }
}