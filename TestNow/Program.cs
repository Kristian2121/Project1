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
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
namespace Program
{
    class Program
    {
        static ITelegramBotClient botClient;

        static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient("7467417422:AAEAAMYecsSPSBnNHQw_dxUkrAOm1Xux7oc");
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hi, my username is {me.Id} my name is {me.FirstName}.");
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Console.WriteLine("Press any key to exist");
            Console.ReadKey();
            botClient.StopReceiving();
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                string wetherInfo = GetWeatherInfo(e.Message.Text);
                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: wetherInfo);
            }

        }

        public async Task GetWeatherInfo(string message)
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\krist\\source\\repos\\TestNow\\TestNow\\TestWetherData.mdf;Integrated Security=True";
            string apiURL = "https://api.openweathermap.org/data/2.5/weather?lat={43.0033}&lon={41.0153}&appid={a3d6d40ea2d819407c5923d19ac5a418}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiURL); ;
                    response.EnsureSuccessStatusCode();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nExceptionCaught!");
                    Console.WriteLine("Message : {0}", e.Message);

                }
                return $"Погода в {}:";
            }
        }

    }
}