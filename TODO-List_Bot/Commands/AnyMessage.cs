using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TODO_List_Bot.Commands;

public class AnyMessage
{

    private static string taskName;
    public static async Task<Message> OnMessageReceived(ITelegramBotClient bot, Message message)
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(
            new[]
            {
                new KeyboardButton[] { "Список тасков" },
                new KeyboardButton[] { "Добавить таск" },
                new KeyboardButton[] { "Настройки" }
            })
        {
            ResizeKeyboard = true
        };

        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: "Выберите",
            replyMarkup: replyKeyboardMarkup);
    }
}