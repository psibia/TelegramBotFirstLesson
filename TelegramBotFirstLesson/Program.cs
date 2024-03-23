using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotFirstLesson
{
    class Program
    {
        static ITelegramBotClient? botClient;
        static long? chatId = null;

        static void Main(string[] args)
        {
            var botToken = "6888489743:AAGQAJRYTN5ZYFjIWpL7T_moSRJEKCkoXOg";
            botClient = new TelegramBotClient(botToken);

            var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);

            Console.WriteLine("Бот запущен. Ожидание сообщений...");
            Console.ReadKey();
            cts.Cancel();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
            {
                chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;
                Console.WriteLine($"Получено сообщение в чате {chatId}: {messageText}");
            }
        }

        static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Произошла ошибка: {exception.Message}");
        }
    }
}