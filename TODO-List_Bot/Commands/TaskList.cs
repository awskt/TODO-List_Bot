using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TODO_List_Bot.Services;

namespace TODO_List_Bot.Commands;

public static class TaskList
{
    public static async Task<Message> SendTaskList(ITelegramBotClient bot, Message message)
    {
        var tasks = HandleUpdateService.tasks;
        Console.WriteLine(tasks.Count);

        if (tasks.Count > 0)
        {
            foreach (var task in tasks)
            {
                Console.WriteLine("sdfsfsdfsdf");
                SendTask(bot, message, task.Name);
            }
        }
        else
        {
            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                text: "Список тасков пуст");
        }
        
        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: "");
    }
    
    private static void SendTask(ITelegramBotClient bot, Message message, string taskName)
    {
        InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                InlineKeyboardButton.WithCallbackData("✅", "Таск " + taskName + " выполнен"),
                InlineKeyboardButton.WithCallbackData("🖋", "Вы хотите изменить таск " + taskName + "? (Да/Нет)"),
                InlineKeyboardButton.WithCallbackData("🚫", "Таск " + taskName + " удален")
            });
    
        bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: taskName,
            replyMarkup: inlineKeyboard);
    }
    
    public static async Task EditTask(ITelegramBotClient bot, Message message, TaskObject task)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup = new(
            new []
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Изменить название", "1")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Изменить дату", "2")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Изменить время", "3")
                }
            });
    
        await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: task.Name,
            replyMarkup: inlineKeyboardMarkup);
    }
}