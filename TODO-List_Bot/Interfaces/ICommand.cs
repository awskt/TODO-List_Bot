using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TODO_List_Bot.Interfaces;

public interface ICommand
{
    Task<Message> SendMessage(ITelegramBotClient bot, Message message);
}