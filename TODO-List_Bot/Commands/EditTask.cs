using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TODO_List_Bot.Interfaces;

namespace TODO_List_Bot.Commands;

public class EditTask : ICommand
{
    public Task<Message> SendMessage(ITelegramBotClient bot, Message message)
    {
        throw new NotImplementedException();
    }
}