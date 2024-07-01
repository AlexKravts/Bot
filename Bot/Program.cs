using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;

namespace BOT
{
    internal class Program
    {
        private static Dictionary<long, int> playerScores = new Dictionary<long, int>();

        static void Main(string[] args)
        {
            var client = new TelegramBotClient("7371882284:AAETtgtfVRcLi_pwuJeEGtxG8XYoe2qNYsI");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message != null && message.Text != null)
                {
                var chatId = message.Chat.Id;
                var chatName = message.Chat.FirstName;
                Console.WriteLine($"{chatName} | {message.Text}");

                if (!playerScores.ContainsKey(chatId))
                {
                    playerScores[chatId] = 50; 
                }

                if (message.Text.ToLower().Contains("/start"))
                {
                    await StartGame(client, chatId, chatName);
                }
                else if (message.Text.ToLower().Contains("✊"))
                {
                    await PlayRock(client, chatId);
                }
                else if (message.Text.ToLower().Contains("✌"))
                {
                    await PlayScissors(client, chatId);
                }
                else if (message.Text.ToLower().Contains("✋"))
                {
                    await PlayPaper(client, chatId);
                }
                else
                {
                    await Unknown(client, chatId, chatName);
                }
            }
        }

        static async Task StartGame(ITelegramBotClient client, long chatId, string chatName)
        {
            await client.SendTextMessageAsync(chatId, $"Привет {chatName} ! Давай поиграем?! Выбери Камень: ✊, Ножницы: ✌, или Бумагу: ✋. У тебя {playerScores[chatId]} очков.", replyMarkup: GetButtons());
        }

        static async Task PlayRock(ITelegramBotClient client, long chatId)
        {
            int variable = 0;
            int variable_bot = new Random().Next(0, 3);
            await SendBotChoice(client, chatId, variable_bot);
            await HandleResult(client, chatId, variable, variable_bot);
        }

        static async Task PlayScissors(ITelegramBotClient client, long chatId)
        {
            int variable = 1;
            int variable_bot = new Random().Next(0, 3);
            await SendBotChoice(client, chatId, variable_bot);
            await HandleResult(client, chatId, variable, variable_bot);
        }

        static async Task PlayPaper(ITelegramBotClient client, long chatId)
        {
            int variable = 2;
            int variable_bot = new Random().Next(0, 3);
            await SendBotChoice(client, chatId, variable_bot);
            await HandleResult(client, chatId, variable, variable_bot);
        }

        static async Task SendBotChoice(ITelegramBotClient client, long chatId, int variable_bot)
        {
            if (variable_bot == 0)
                await client.SendTextMessageAsync(chatId, $"У меня Камень: ✊", replyMarkup: GetButtons());
            else if (variable_bot == 1)
                await client.SendTextMessageAsync(chatId, $"У меня Ножницы: ✌", replyMarkup: GetButtons());
            else if (variable_bot == 2)
                await client.SendTextMessageAsync(chatId, $"У меня Бумага: ✋", replyMarkup: GetButtons());
        }

        static async Task HandleResult(ITelegramBotClient client, long chatId, int variable, int variable_bot)
        {
            if (variable == variable_bot)
                await client.SendTextMessageAsync(chatId, "Ничья", replyMarkup: GetButtons());
            else if ((variable == 0 && variable_bot == 1) || (variable == 1 && variable_bot == 2) || (variable == 2 && variable_bot == 0))
            {
                playerScores[chatId] += 10;
                await client.SendTextMessageAsync(chatId, $"Результат: Ты победил! и заработал 10 очков, теперь у тебя {playerScores[chatId]} очков", replyMarkup: GetButtons());
            }
            else
            {
                playerScores[chatId] -= 10;
                await client.SendTextMessageAsync(chatId, $"Результат: Я победил! и забираю у тебя 10 очков, теперь у тебя {playerScores[chatId]} очков ", replyMarkup: GetButtons());
            }
        }

        static async Task Unknown(ITelegramBotClient client, long chatId, string chatName)
        {
            await client.SendTextMessageAsync(chatId, $"Неизвестное значение!!! {chatName} Выбери что то из трех вариантов!", replyMarkup: GetButtons());
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("✊"),
                    new KeyboardButton("✌"),
                    new KeyboardButton("✋")
                }
            });
        }

        private static async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
        }
    }
}