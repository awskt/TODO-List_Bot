using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TODO_List_Bot.Services;

public class HandleUpdateService
{
    private static IMemoryCache _cache;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<HandleUpdateService> _logger;

    private static List<TaskObject> tasks = new();
    
    public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger, IMemoryCache memoryCache)
    {
        _botClient = botClient;
        _logger = logger;
        _cache = memoryCache;
    }

    public async Task EchoAsync(Update update)
    {
        var handler = update.Type switch
        {
            UpdateType.Message            => BotOnMessageReceived(update.Message!),
            UpdateType.EditedMessage      => BotOnMessageReceived(update.EditedMessage!),
            UpdateType.CallbackQuery      => BotOnCallbackQueryReceived(update.CallbackQuery!),
            UpdateType.InlineQuery        => BotOnInlineQueryReceived(update.InlineQuery!),
            UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult!),
            _                             => UnknownUpdateHandlerAsync(update)
        };

        try
        {
            await handler;
        }
        #pragma warning disable CA1031
        catch (Exception exception)
        #pragma warning restore CA1031
        {
            await HandleErrorAsync(exception);
        }
    }

    private async Task BotOnMessageReceived(Message message)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Type != MessageType.Text)
            return;

        var action = message.Text! switch
        {
            "Список тасков" => SendTaskList(_botClient, message),
            "Добавить таск" => AddTask(_botClient, message),
            _ => SendMenu(_botClient, message)
        };
        Message sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

    }

    private async Task<Message> SendMenu(ITelegramBotClient bot, Message message)
    {
        string cacheMsg;
        if (_cache.TryGetValue("lastMessage", out cacheMsg))
        {
            var taskName = message.Text;
            if (taskName != "Добавить таск" && cacheMsg == "Добавить таск")
            {
                tasks.Add(new TaskObject(taskName));
                _cache.Remove("lastMessage");
            }
            
            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                text: "Таск " + taskName + " успешно добавлен");
        }

        
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
    
    static async Task<Message> SendTaskList(ITelegramBotClient bot, Message message)
    {
        if (tasks.Count > 0)
        {
            foreach (var task in tasks)
            {
                SendTaskArray(bot, message, task.Name);
            }  
        }
        else
        {
            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                text: "Список тасков пуст");
        }
        
        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: null);
    }

    static async Task<Message> SendTaskArray(ITelegramBotClient bot, Message message, string taskName)
    {
        InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                InlineKeyboardButton.WithCallbackData("✅", "11"),
                InlineKeyboardButton.WithCallbackData("🖋", "12"),
                InlineKeyboardButton.WithCallbackData("🚫", "12")
            });

        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: taskName,
            replyMarkup: inlineKeyboard);
    } 

    static async Task<Message> AddTask(ITelegramBotClient bot, Message message)
    {
        _cache.Set("lastMessage", "Добавить таск");

        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
            text: "Введите название таска:");
    }

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        await _botClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: $"Received {callbackQuery.Data}");

        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            text: $"Received {callbackQuery.Data}");
    }

    #region Inline Mode

    private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
    {
        _logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "3",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent(
                    "hello"
                )
            )
        };

        await _botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                results: results,
                                                isPersonal: true,
                                                cacheTime: 0);
    }

    private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
    {
        _logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);
        return Task.CompletedTask;
    }

    #endregion

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }
}