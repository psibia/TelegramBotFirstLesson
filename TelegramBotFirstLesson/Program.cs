using System.IO.Ports;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotFirstLesson
{
    class Program
    {
        static ITelegramBotClient? botClient;
        static long? allowedChatId = 897988198;
        static SerialPort serialPort = new SerialPort("COM4", 9600);

        static void Main(string[] args)
        {
            var botToken = "6329542606:AAHiT5fZfSDi37yH7C7NCX2dWQqfmOIdsjQ";
            botClient = new TelegramBotClient(botToken);

            var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            serialPort.Open();
            serialPort.DataReceived += SerialPort_DataReceived;

            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);

            Console.WriteLine("Бот и Arduino запущены. Ожидание сообщений и данных...");
            Console.ReadKey();
            cts.Cancel();
            serialPort.Close();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;
                Console.WriteLine($"Получено сообщение в чате {chatId}: {messageText}");

                if (chatId == allowedChatId)
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.WriteLine(messageText);
                        Console.WriteLine("Сообщение отправлено на Arduino.");
                    }
                    else
                    {
                        Console.WriteLine("SerialPort не открыт.");
                    }
                }
                else
                {
                    Console.WriteLine($"Сообщение от неавторизованного пользователя {chatId}. Игнорируется.");
                }
            }
        }

        static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Произошла ошибка: {exception.Message}");
        }

        // Метод обработки данных, полученных из серийного порта
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = serialPort.ReadLine();
            Console.WriteLine($"Получены данные с Arduino: {data}");

            // Отправляем полученные данные обратно в Telegram
            try
            {
                botClient.SendTextMessageAsync(allowedChatId, data).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке данных в Telegram: {ex.Message}");
            }
        }
    }
}