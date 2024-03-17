using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
using langLocal = MM_Helper_in_TG.Properties.Languages;


var token = "YOUR_BOT_TOKEN";

// Створіть об'єкт botClient
var botClient = new TelegramBotClient(token);
using CancellationTokenSource cts = new();


string? exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
string? exeDir = Path.GetDirectoryName(path: exePath);

//Дати перевірки для серверів 116 та 127
DateTime CheckDate116 = new DateTime(2023, 6, 10);
DateTime CheckDate127 = new DateTime(2023, 6, 11);

//Контрольна дата для оновлення ретингів 
DateTime CheckRanking = new DateTime(2023, 6, 12, 8, 0, 0);


//Встановлюємо актуальну дату та час в годинах та хвилинах
string currentTime = DateTime.Now.ToString("H:mm");

//Встановлюємо час для надходження повідомлень про початок тимчасових подій 
//Ескорт каравану
List<string> CaravanEscortTime = new List<string> { "16:00", "22:00", "4:00" };
List<string> CaravanEscortPre5Time = new List<string> { "15:55", "21:55", "3:55" };

//Острівна битва
List<string> IslandBattleTime = new List<string> { "18:00" };
List<string> IslandBattlePre5Time = new List<string> { "17:55" };

//Молитва Священому дереву
List<string> HolyTreeBlessingTimeMorn = new List<string> { "8:00" };
List<string> HolyTreeBlessingTime = new List<string> { "18:00" };


//Встановлюємо час для надходження повідомлень про початок щоденних подій 
List<string> GASTime = new List<string> { "4:00", "12:00", "20:00" };
List<string> MATime = new List<string> { "00:01", "8:00", "16:00", "24:00" };
List<string> ShopTime = new List<string> { "00:00", "2:00", "4:00", "6:00", "8:00", "10:00", "12:00", "14:00", "16:00", "18:00", "20:00", "22:00"};





int RanNow;
int RanNext;
int EveNow;
int EveNext;

string? EveNowText1 = default;
string? EveNowText2 = default;
string? EveNowText3 = default;

string? EveNextText1 = default;
string? EveNextText2 = default;
string? EveNextText3 = default;



Dictionary<ChatId, string>? allUsers = LoadUsers("allUsers");
Dictionary<ChatId, string>? eventNotifUsers = LoadUsers("eventNotifUsers");
Dictionary<ChatId, string>? banquetShopUsers = LoadUsers("banquetShopUsers");
Dictionary<ChatId, string>? MAUsers = LoadUsers("MAUsers");
Dictionary<ChatId, string>? GASUsers = LoadUsers("GASUsers");




string language = "en"; // Replace with user's language



await botClient.SetMyNameAsync(langLocal.Localization.botName, default, default);
await botClient.SetMyDescriptionAsync(langLocal.Localization.botDescription, default, default);
await botClient.SetMyShortDescriptionAsync(langLocal.Localization.botShortDescription, default, default);



// Set the bot commands
// Встановлюємо області видимості для команд

BotCommandScopeAllPrivateChats scoreAllPrivateChats = new BotCommandScopeAllPrivateChats(); // Всі приватні чати
BotCommandScopeAllGroupChats scoreAllGroupChats = new BotCommandScopeAllGroupChats(); // Участники всіх груп та супергруп
BotCommandScopeAllChatAdministrators scopeAllChatAdministrators = new BotCommandScopeAllChatAdministrators(); // Адміністратори всіх груп та супергруп

// Створюємо команди для всіх приватних груп
List<BotCommand> commandsAllPrivateChats = new List<BotCommand>
{
    new BotCommand { Command = "shorthelp", Description = langLocal.Localization.shortHelp},
    new BotCommand { Command = "help", Description = langLocal.Localization.help },
    new BotCommand { Command = "ratingsnow", Description = langLocal.Localization.ratingsNow },
    new BotCommand { Command = "ratingsnext", Description = langLocal.Localization.ratingsNext },
    new BotCommand { Command = "eventsnow", Description = langLocal.Localization.eventsNow },
    new BotCommand { Command = "eventsnext", Description = langLocal.Localization.eventsNext },
    new BotCommand { Command = "eventsnotif", Description = langLocal.Localization.eventsNotif },
    new BotCommand { Command = "guildnotif", Description = langLocal.Localization.guildNotif },
    new BotCommand { Command = "banqnotif", Description = langLocal.Localization.banqNotif },
    new BotCommand { Command = "gasnotif", Description = langLocal.Localization.gasNotif },
    new BotCommand { Command = "manotif", Description = langLocal.Localization.maNotif },
    new BotCommand { Command = "srnotif", Description = langLocal.Localization.srNotif }
};

// Створюємо команди для участників всіх груп та супергруп
List<BotCommand> commandsAllGroupChats = new List<BotCommand>
{
    new BotCommand { Command = "shorthelp", Description = langLocal.Localization.shortHelp },
    new BotCommand { Command = "help", Description = langLocal.Localization.help },
    new BotCommand { Command = "ratingsnow", Description = langLocal.Localization.ratingsNow },
    new BotCommand { Command = "ratingsnext", Description = langLocal.Localization.ratingsNext },
    new BotCommand { Command = "eventsnow", Description = langLocal.Localization.eventsNow },
    new BotCommand { Command = "eventsnext", Description = langLocal.Localization.eventsNext }
};

// Створюємо команди для адміністраторів всіх груп та супергруп
List<BotCommand> commandsAllChatAdministrators = new List<BotCommand>
{
    new BotCommand { Command = "shorthelp", Description = langLocal.Localization.shortHelp},
    new BotCommand { Command = "help", Description = langLocal.Localization.help },
    new BotCommand { Command = "ratingsnow", Description = langLocal.Localization.ratingsNow },
    new BotCommand { Command = "ratingsnext", Description = langLocal.Localization.ratingsNext },
    new BotCommand { Command = "eventsnow", Description = langLocal.Localization.eventsNow },
    new BotCommand { Command = "eventsnext", Description = langLocal.Localization.eventsNext },
    new BotCommand { Command = "eventsnotif", Description = langLocal.Localization.eventsNotif },
    new BotCommand { Command = "guildnotif", Description = langLocal.Localization.guildNotif },
    new BotCommand { Command = "banqnotif", Description = langLocal.Localization.banqNotif },
    new BotCommand { Command = "gasnotif", Description = langLocal.Localization.gasNotif },
    new BotCommand { Command = "manotif", Description = langLocal.Localization.maNotif },
    new BotCommand { Command = "srnotif", Description = langLocal.Localization.srNotif }
};

// Встановлюємо команди 
await botClient.SetMyCommandsAsync(commandsAllPrivateChats, scoreAllPrivateChats, default);
await botClient.SetMyCommandsAsync(commandsAllGroupChats, scoreAllGroupChats, default);
await botClient.SetMyCommandsAsync(commandsAllChatAdministrators, scopeAllChatAdministrators, default);


DateTime WorldCommerce = new DateTime(2023, 5, 31, 8, 0, 0);
string WorldCommerceText = langLocal.Localization.WorldCommerceText;

Dictionary<string, DateTime> events54days = new Dictionary<string, DateTime> 
{
    {langLocal.Localization.MooncakeText, new DateTime(2023, 6, 6, 8, 0, 0)},
    {langLocal.Localization.MonopolyCarnivalText, new DateTime(2023, 6, 12, 8, 0, 0)},
    {langLocal.Localization.WonderfulMelodyText, new DateTime(2023, 6, 18, 8, 0, 0)},
    {langLocal.Localization.IslandBattleText, new DateTime(2023, 5, 1, 8, 0, 0)},
    {langLocal.Localization.ABoxOfFruitText, new DateTime(2023, 5, 7, 8, 0, 0)},
    {langLocal.Localization.CaravanEscortText, new DateTime(2023, 5, 13, 8, 0, 0)},
    {langLocal.Localization.TombTreasureText, new DateTime(2023, 5, 19, 8, 0, 0)}    
};

Dictionary<string, DateTime> events30days = new Dictionary<string, DateTime>
{
    {langLocal.Localization.BazaarTreasureText, new DateTime(2023, 5, 28, 8, 0, 0)},
    {langLocal.Localization.HolyTreeBlessingText, new DateTime(2023, 5, 22, 8, 0, 0)},
    {langLocal.Localization.MidAutumnDiceGameText, new DateTime(2023, 5, 16, 8, 0, 0)},
    {langLocal.Localization.WishdomScholarText, new DateTime(2023, 5, 10, 8, 0, 0)},
    {langLocal.Localization.CoinDivinationText, new DateTime(2023, 5, 4, 8, 0, 0)}
};

Dictionary<string, DateTime> events48days = new Dictionary<string, DateTime>
{
    {langLocal.Localization.CookingTurkeyText, new DateTime(2023, 4, 16, 8, 0, 0)},
    {langLocal.Localization.MonopolyText, new DateTime(2023, 7, 21, 8, 0, 0)},
    {langLocal.Localization.CatchText,  new DateTime(2023, 7, 24, 8, 0, 0)},
    {langLocal.Localization.SpringTowerText, new DateTime(2023, 7, 27, 8, 0, 0)},
    {langLocal.Localization.DecoupageText, new DateTime(2023, 7, 30, 8, 0, 0)},
    {langLocal.Localization.BeautifulBrushText, new DateTime(2023, 8, 2, 8, 0, 0)},
    {langLocal.Localization.ThanksgivingFestivalText,  new DateTime(2023, 8, 5, 8, 0, 0)},
    {langLocal.Localization.WeedingMasterText, new DateTime(2023, 8, 8, 8, 0, 0)},
    {langLocal.Localization.MasterArcheryText, new DateTime(2023, 8, 11, 8, 0, 0)},
    {langLocal.Localization.CrowsBridgeText, new DateTime(2023, 8, 14, 8, 0, 0)},
    {langLocal.Localization.SwordsmithText, new DateTime(2023, 8, 17, 8, 0, 0)},
    {langLocal.Localization.CristmasPainingText, new DateTime(2023, 8, 20, 8, 0, 0)},
    {langLocal.Localization.WrapGiftsText, new DateTime(2023, 8, 23, 8, 0, 0)},
    {langLocal.Localization.FloverTeaText, new DateTime(2023, 8, 26, 8, 0, 0)},
    {langLocal.Localization.HalloweenGhostFireText, new DateTime(2023, 8, 26, 8, 0, 0)},
    {langLocal.Localization.TheMatchingGameText, new DateTime(2023, 8, 29, 8, 0, 0)},
    {langLocal.Localization.FlowerForBeloverText, new DateTime(2023, 9, 1, 8, 0, 0)}
};



// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);



var me = await botClient.GetMeAsync();

Console.WriteLine($"I am user {me.Id} and my name is {me.FirstName}.");



while (true)
{
    if (GASTime.Contains(currentTime))
    {
        Parallel.ForEach(GASUsers.Keys, async user =>
        {
            try
            {
                string TextMessage = langLocal.Localization.TextMessageGAS;

                await botClient.SendTextMessageAsync(
                    chatId: user,
                    text: TextMessage,
                    parseMode: ParseMode.Html
                );
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 403)
            {
                Console.WriteLine(ex);
            }
        });
    }

    if (MATime.Contains(currentTime))
    {
        Parallel.ForEach(MAUsers.Keys, async user =>
        {
            try
            {
                string TextMessage = langLocal.Localization.TextMessageMA;

                await botClient.SendTextMessageAsync(
                    chatId: user,
                    text: TextMessage,
                    parseMode: ParseMode.Html
                );
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 403)
            {
                Console.WriteLine(ex);
            }
        });
    }
    Thread.Sleep(60000);


}



async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(client, update, cancellationToken);
        return;
    }
    
    if (!(update.Message is Message message) || !(message.Text is string messageText))
        return;

    if ((DateTime.UtcNow - message.Date).TotalMinutes > 1)
        return;

    var chatId = message.Chat.Id;
    string languageCode = message.From.LanguageCode;

    if (!allUsers.TryGetValue(chatId, out var existingLanguage) || existingLanguage != language)
    {
        allUsers[chatId] = languageCode;
        SaveUsers(allUsers, "allUsers");
    }

    await HandleComamnd(client, update, cancellationToken);

}

async Task HandleComamnd (ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    if (!(update.Message is Message message) || !(message.Text is string messageText))
        return;

    switch (messageText)
    {
        case "/start":
            await greatingMessage(client, update, cancellationToken, language);
            break;
        case "/shorthelp":
            await shortHelpMessage(client, update, cancellationToken, language);
            break;
        case "/help":
            await helpMessage(client, update, cancellationToken, language);
            break;
        case "/ratingsnow":
            await ratingsNowMessage(client, update, cancellationToken, language);
            break;
        case "/ratingsnext":
            await ratingsNextMessage(client, update, cancellationToken, language);
            break;
        case "/eventsnow":
            await eventsNowMessage(client, update, cancellationToken, language);
            break;
        case "/eventsnext":
            await eventsNextMessage(client, update, cancellationToken, language);
            break;
        case "/eventsnotif":
            await eventsNotifMessage(client, update, cancellationToken, language);
            break;
        case "/banqnotif":
            await BanquetShopMessage(client, update, cancellationToken, language);
            break;
        case "/gasnotif":
            await GASMessage(client, update, cancellationToken, language);
            break;
        case "/manotif":
            await MAMessage(client, update, cancellationToken, language);
            break;
        case "/srnotif":

            break;
    }
}

async Task HandleCallbackQuery(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{    
    CallbackQuery? callbackQuery = update.CallbackQuery;

    
    string Menu = langLocal.Localization.Menu;
    
    string Rating = langLocal.Localization.Rating;    
    string RatingNow = langLocal.Localization.RatingNow;    
    string RatingNext = langLocal.Localization.RatingNext;
    
    string Event = langLocal.Localization.Event;    
    string EventNow = langLocal.Localization.EventNow;    
    string EventNext = langLocal.Localization.EventNext;    
    string EventNotif = langLocal.Localization.EventNotif;
    
    string Guild = langLocal.Localization.Guild;    
    string BanqShop = langLocal.Localization.BanqShop;    
    string GAS = langLocal.Localization.GAS;    
    string MA = langLocal.Localization.MA;    
    string ShopingRow = langLocal.Localization.ShopingRow;
    
    string HideMenu = langLocal.Localization.HideMenu;    
    string BackToMenu = langLocal.Localization.BackToMenu;



    InlineKeyboardMarkup inlineKeyboardMenuFull = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(Rating) },
        new [] { InlineKeyboardButton.WithCallbackData(Event) },
        new [] { InlineKeyboardButton.WithCallbackData(BanqShop) },
        new [] { InlineKeyboardButton.WithCallbackData(GAS) },
        new [] { InlineKeyboardButton.WithCallbackData(MA) },
        new [] { InlineKeyboardButton.WithCallbackData(HideMenu) }
    });

    InlineKeyboardMarkup inlineKeyboardRatingMenu = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(RatingNow) },
        new [] { InlineKeyboardButton.WithCallbackData(RatingNext) },
        new [] { InlineKeyboardButton.WithCallbackData(BackToMenu) }
    });

    InlineKeyboardMarkup inlineKeyboardEventsMenu = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(EventNow) },
        new [] { InlineKeyboardButton.WithCallbackData(EventNext) },
        new [] { InlineKeyboardButton.WithCallbackData(EventNotif) },
        new [] { InlineKeyboardButton.WithCallbackData(BackToMenu) }
    });


    InlineKeyboardMarkup inlineKeyboardMenu = new InlineKeyboardMarkup(new[]
    {
         new [] { InlineKeyboardButton.WithCallbackData(Menu) },
    });

    

    if (callbackQuery.Data.Equals(Menu))
    {
        //Зчитує текст для повідомлення        
        string shortHelpText = langLocal.Localization.shortMenu;

        await inlineKeyboardUpdateText(client, callbackQuery, cancellationToken, inlineKeyboardMenuFull, shortHelpText);
        return;
    }

    if (callbackQuery.Data.Equals(Rating))
    {
        await inlineKeyboardMessage(client, callbackQuery, cancellationToken, inlineKeyboardRatingMenu);
        return;
    }

    if (callbackQuery.Data.Equals(RatingNow))
    {
        await ratingsNowMessage(client, update, cancellationToken, language);
        botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: cancellationToken);
        return;
    }

    if (callbackQuery.Data.Equals(RatingNext))
    {
        await ratingsNextMessage(client, update, cancellationToken, language);
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(Event))
    {
        await inlineKeyboardMessage(client, callbackQuery, cancellationToken, inlineKeyboardEventsMenu);
        return;
    }

    if (callbackQuery.Data.Equals(EventNow))
    {
        await eventsNowMessage(client, update, cancellationToken, language);
        botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(EventNext))
    {
        await eventsNextMessage(client, update, cancellationToken, language);
        botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(EventNotif))
    {
        await eventsNotifMessage(client, update, cancellationToken, language);
        botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }



    if (callbackQuery.Data.Equals(BanqShop))
    {
        await BanquetShopMessage(client, update, cancellationToken, language);
        botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(GAS))
    {
        await GASMessage(client, update, cancellationToken, language);
        botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(MA))
    {
        await MAMessage(client, update, cancellationToken, language);
        botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }



    if (callbackQuery.Data.Equals(BackToMenu))
    {
        await inlineKeyboardMessage(client, callbackQuery, cancellationToken, inlineKeyboardMenuFull);
        return;
    }

    if (callbackQuery.Data.Equals(HideMenu))
    {
        await inlineKeyboardMessage(client, callbackQuery, cancellationToken, inlineKeyboardMenu);
        return;
    }
    return;
    
}

async Task inlineKeyboardUpdateText(ITelegramBotClient client, CallbackQuery callbackQuery, CancellationToken cancellationToken, InlineKeyboardMarkup inlineKeyboard, string updateText)
{
    await botClient.EditMessageTextAsync(
           chatId: callbackQuery.Message.Chat.Id,
           text: updateText,
           messageId: callbackQuery.Message.MessageId,
           replyMarkup: inlineKeyboard,
           cancellationToken: cancellationToken
    );
}

async Task inlineKeyboardMessage(ITelegramBotClient client, CallbackQuery callbackQuery, CancellationToken cancellationToken, InlineKeyboardMarkup inlineKeyboard)
{
    await botClient.EditMessageReplyMarkupAsync(
            chatId: callbackQuery.Message.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken
    );
}



async Task MAMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Визначаємо тип оновлення
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    //Зчитує текст для повідомлення
    string filePathMAStart = Path.Combine(exeDir, $"{language}\\BanqStart.txt");
    string MAStartText = File.ReadAllText(filePathMAStart);

    string filePathMAStop = Path.Combine(exeDir, $"{language}\\BanqStop.txt");
    string MAStopText = File.ReadAllText(filePathMAStop);

    string messageText = null;

    if (!MAUsers.ContainsKey(chatId))
    {
        //Надсилаємо повідомлення
        messageText = MAStartText;
        MAUsers.Add(chatId, language);
    }
    else
    {
        //Надсилаємо повідомлення
        messageText = MAStopText;
        MAUsers.Remove(chatId);
    }
    SaveUsers(MAUsers, "MAUsers");

    await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: messageText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: messageId,
        cancellationToken: cancellationToken
        );
    return;
}

async Task GASMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Визначаємо тип оновлення
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    //Зчитує текст для повідомлення
    string filePathGASStart = Path.Combine(exeDir, $"{language}\\GASStart.txt");
    string GASStartText = File.ReadAllText(filePathGASStart);

    string filePathGASStop = Path.Combine(exeDir, $"{language}\\GASStop.txt");
    string GASStopText = File.ReadAllText(filePathGASStop);

    string messageText = null;

    if (!GASUsers.ContainsKey(chatId))
    {
        //Надсилаємо повідомлення
        messageText = GASStartText;
        GASUsers.Add(chatId, language);
    }
    else
    {
        //Надсилаємо повідомлення
        messageText = GASStopText;
        GASUsers.Remove(chatId);
    }
    SaveUsers(GASUsers, "GASUsers");

    await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: messageText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: messageId,
        cancellationToken: cancellationToken
        );
    return;
}

async Task BanquetShopMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Визначаємо тип оновлення
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    //Зчитує текст для повідомлення
    string filePathBanquetShopStart = Path.Combine(exeDir, $"{language}\\BanqStart.txt");
    string BanquetShopStartText = File.ReadAllText(filePathBanquetShopStart);

    string filePathBanquetShopStop = Path.Combine(exeDir, $"{language}\\BanqStop.txt");
    string BanquetShopStopText = File.ReadAllText(filePathBanquetShopStop);

    string messageText = null;

    if (!banquetShopUsers.ContainsKey(chatId))
    {
        //Надсилаємо повідомлення
        messageText = BanquetShopStartText;
        banquetShopUsers.Add(chatId, language);
    }
    else
    {
        //Надсилаємо повідомлення
        messageText = BanquetShopStopText;
        banquetShopUsers.Remove(chatId);
    }
    SaveUsers(banquetShopUsers, "BanquetShopUsers");

    await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: messageText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: messageId,
        cancellationToken: cancellationToken
        );
    return;
}

async Task eventsNotifMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Визначаємо тип оновлення
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    //Зчитує текст для повідомлення
    string filePathEventsNotifStart = Path.Combine(exeDir, $"{language}\\event\\NotifStart.txt");
    string EventsNotifStartText = File.ReadAllText(filePathEventsNotifStart);

    string filePathEventsNotifStop = Path.Combine(exeDir, $"{language}\\event\\NotifStop.txt");
    string EventsNotifStopText = File.ReadAllText(filePathEventsNotifStop);

    string messageText = null;

    if (!eventNotifUsers.ContainsKey(chatId))
    {
        //Надсилаємо повідомлення
        messageText = EventsNotifStartText;
        eventNotifUsers.Add(chatId, language);
    }
    else
    {
        //Надсилаємо повідомлення
        messageText = EventsNotifStopText;
        eventNotifUsers.Remove(chatId);
    }
    SaveUsers(eventNotifUsers, "eventNotifUsers");

    await botClient.SendTextMessageAsync(            
        chatId: chatId,
        text: messageText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: messageId,
        cancellationToken: cancellationToken
        );
    return;
}

void SaveUsers(Dictionary<ChatId, string> dictionary, string nameFile)
{
    string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
    string filePath = Path.Combine(basePath, "Users", $"{nameFile}.json");

    try
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Спроба створити директорію
        string json = JsonConvert.SerializeObject(dictionary);
        File.WriteAllText(filePath, json);
    }
    catch (Exception ex)
    {
        // Обробка можливих помилок при створенні директорії або запису в файл
        Console.WriteLine($"Помилка при запису в файл: {ex.Message}");
        // Можливо, потрібно виконати додаткову обробку помилки або повідомити користувача.
    }

}

Dictionary<ChatId, string>? LoadUsers(string nameFile)
{

    string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
    string filePath = Path.Combine(basePath, "Users", $"{nameFile}.json");

    if (File.Exists(filePath))
    {
        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Dictionary<ChatId, string>>(json);
    }

    return new Dictionary<ChatId, string>();
}

async Task eventsNextMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Визначаємо тип оновлення
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    //Викликаємо метод розрахунку 3 днів та подій
    counterThreeDays();
    checkingEvents(language);

    string[] ENextText = new string[4];
    string[] imagenEveNext = new string[4];

    //Зчитуємо текст для повідомлення
    string fileEventNext = Path.Combine(exeDir, $"{language}\\event\\eventNext.txt");
    string EventNext = File.ReadAllText(fileEventNext);

    string fileEveNextText0 = Path.Combine(exeDir, $"{language}\\event\\eventTextNext{EveNow}.txt");
    ENextText[0] = File.ReadAllText(fileEveNextText0);

    if (EveNextText1 != null) 
    { 
        ENextText[1] = File.ReadAllText(EveNextText1); 

    }
    if (EveNextText2 != null) 
    { 
        ENextText[2] = File.ReadAllText(EveNextText2);

    }
    if (EveNextText3 != null) 
    { 
        ENextText[3] = File.ReadAllText(EveNextText3);

    }

    //Зчитуємо зображення для повідомлення
    string fileEveNextImg0 = Path.Combine(exeDir, $"img\\eventImage{EveNext}.png");
    var imagenEveNext0 = InputFile.FromStream(File.OpenRead(fileEveNextImg0));

    //Надсилаємо повідомлення
    await botClient.SendPhotoAsync(
        chatId: chatId,
        photo: imagenEveNext0,
        caption:
        $"{EventNext}\n" +
        $"\n" +
        $"{ENextText[0]}\n" +
        $"\n" +
        $"{ENextText[1]}\n" +
        $"\n" +
        $"{ENextText[2]}\n" +
        $"\n" +
        $"{ENextText[3]}\n",
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: messageId,
        cancellationToken: cancellationToken
        );
    return;
}

async Task eventsNowMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Визначаємо тип оновлення
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    //Викликаємо метод розрахунку 3 днів та подій
    counterThreeDays();
    checkingEvents(language);

    string[] ENowText = new string[4];

    //Зчитуємо текст для повідомлення
    string fileEventNow = Path.Combine(exeDir, $"{language}\\event\\eventNow.txt");
    string EventNow = File.ReadAllText(fileEventNow);

    string fileEveNowText0 = Path.Combine(exeDir, $"{language}\\event\\eventTextNow{EveNow}.txt");
    ENowText[0] = File.ReadAllText(fileEveNowText0);
    
    if (EveNowText1 != null || EveNowText1 != default) { ENowText[1] = File.ReadAllText(EveNowText1);}
    if (EveNowText2 != null || EveNowText2 != default) { ENowText[2] = File.ReadAllText(EveNowText2);}
    if (EveNowText3 != null || EveNowText3 != default) { ENowText[3] = File.ReadAllText(EveNowText3);}
    
    //Зчитуємо зображення для повідомлення
    string fileEveNowImg0 = Path.Combine(exeDir, $"img\\eventImage{EveNow}.png");
    var imagenEveNow0 = InputFile.FromStream(File.OpenRead(fileEveNowImg0));
        
    //Надсилаємо повідомлення
    await botClient.SendPhotoAsync(
        chatId: chatId,
        photo: imagenEveNow0,
        caption: 
        $"{EventNow}\n" +
        $"\n" +
        $"{ENowText[0]}\n" +
        $"\n" +
        $"{ENowText[1]}\n" +
        $"\n" +
        $"{ENowText[2]}\n" +
        $"\n" +
        $"{ENowText[3]}\n",
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: messageId,
        cancellationToken: cancellationToken
        );
    return;
}

void checkingEvents(string language)
{
    while (WorldCommerce.AddDays(54) <= DateTime.Now)
    {
        WorldCommerce = WorldCommerce.AddDays(54);

    }
    if (WorldCommerce <= DateTime.Now && WorldCommerce.AddDays(3) >= DateTime.Now)
    {

        String WC = WorldCommerceText;
        if (EveNowText1 == null) { EveNowText1 = WC; }
        else if (EveNowText2 == null && EveNowText1 != WC) { EveNowText2 = WC; }
    }
    if (WorldCommerce.AddDays(54) <= DateTime.Now.AddDays(3) && WorldCommerce.AddDays(57) >= DateTime.Now.AddDays(3))
    {
        String WC = WorldCommerceText;
        if (EveNextText1 == null) { EveNextText1 = WC; }
        else if (EveNextText2 == null && EveNextText1 != WC) { EveNextText2 = WC; }        
    }


    
    foreach (var eventName in events54days.Keys.ToList())
    {
        DateTime eventDateTime = events54days[eventName];
        while (eventDateTime.AddDays(54) <= DateTime.Now)
        {
            eventDateTime = eventDateTime.AddDays(54);
            events54days[eventName] = eventDateTime;
        }
        if (eventDateTime <= DateTime.Now && eventDateTime.AddDays(3) >= DateTime.Now)
        {
            String PathFile = Path.Combine(exeDir, $"{language}\\eventSpec\\{eventName}.txt");
            if (EveNowText1 == null) { EveNowText1 = PathFile; }
            else if (EveNowText2 == null) { EveNowText2 = PathFile; }
        }
        if (eventDateTime.AddDays(54) <= DateTime.Now.AddDays(3) && eventDateTime.AddDays(57) >= DateTime.Now.AddDays(3))
        {
            String PathFile = Path.Combine(exeDir, $"{language}\\eventSpec\\{eventName}.txt");
            if (EveNextText1 == null) { EveNextText1 = PathFile; }
            else if (EveNextText2 == null) { EveNextText2 = PathFile; }            
        }
    }



    foreach (var eventName in events30days.Keys.ToList())
    {
        DateTime eventDateTime = events30days[eventName];
        while (eventDateTime.AddDays(30) <= DateTime.Now)
        {
            eventDateTime = eventDateTime.AddDays(30);
            events30days[eventName] = eventDateTime;
        }
        if (eventDateTime <= DateTime.Now && eventDateTime.AddDays(3) >= DateTime.Now)
        {
            String PathFile = Path.Combine(exeDir, $"{language}\\eventSpec\\{eventName}.txt");
            if (EveNowText1 == null) { EveNowText1 = PathFile; }
            else if (EveNowText2 == null) { EveNowText2 = PathFile; }
            else if (EveNowText3 == null) { EveNowText3 = PathFile; }            
        }
        if (eventDateTime.AddDays(30) <= DateTime.Now.AddDays(3) && eventDateTime.AddDays(33) >= DateTime.Now.AddDays(3))
        {
            String PathFile = Path.Combine(exeDir, $"{language}\\eventSpec\\{eventName}.txt");
            if (EveNextText1 == null) { EveNextText1 = PathFile; }
            else if (EveNextText2 == null) { EveNextText2 = PathFile; }            
        }
    }



    foreach (var eventName in events48days.Keys.ToList())
    {
        DateTime eventDateTime = events48days[eventName];
        while (eventDateTime.AddDays(48) <= DateTime.Now)
        {
            eventDateTime = eventDateTime.AddDays(48);
            events48days[eventName] = eventDateTime;
        }
        if (eventDateTime <= DateTime.Now && eventDateTime.AddDays(3) >= DateTime.Now)
        {
            String PathFile = Path.Combine(exeDir, $"{language}\\eventSpec\\{eventName}.txt");
            if (EveNowText1 == null) { EveNowText1 = PathFile; }
            else if (EveNowText2 == null) { EveNowText2 = PathFile; }            
        }
        if (eventDateTime.AddDays(48) <= DateTime.Now.AddDays(3) && eventDateTime.AddDays(51) >= DateTime.Now.AddDays(3))
        {
            String PathFile = Path.Combine(exeDir, $"{language}\\eventSpec\\{eventName}.txt");
            if (EveNextText1 == null) { EveNextText1 = PathFile; }
            else if (EveNextText2 == null) { EveNextText2 = PathFile; }            
        }
    }
}

async Task ratingsNextMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Визначаємо тип оновлення
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    //Викликаємо метод розрахунку 3 днів та подій
    counterThreeDays();

    //Зчитує текст для повідомлення
    string filePathRanNext = Path.Combine(exeDir, $"{language}\\rating\\textRankingNext{RanNext}.txt");
    string ratingsNextText = File.ReadAllText(filePathRanNext);

    //Надсилаємо повідомлення
    await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: ratingsNextText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: messageId,
        cancellationToken: cancellationToken
        );
    return;

}

async Task ratingsNowMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Визначаємо тип оновлення
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    //Викликаємо метод розрахунку
    counterThreeDays();

    //Зчитує текст для повідомлення
    string filePathRanNow = Path.Combine(exeDir, $"{language}\\rating\\textRankingNow{RanNow}.txt");
    string ratingsNowText = File.ReadAllText(filePathRanNow);
        
    //Надсилаємо повідомлення
    await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: ratingsNowText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: messageId,
        cancellationToken: cancellationToken
        );
    return;

}

void counterThreeDays()
{
    int RanNowProc = 0;
    int RanNextProc = 0;

    while ((CheckRanking + TimeSpan.FromDays(3)) <= DateTime.Now)
    {
        CheckRanking = CheckRanking.AddDays(3);

        RanNowProc = RanNowProc - 1;

        RanNowProc = RanNowProc == -1 ? 14 : RanNowProc;

        EveNowText1 = default;
        EveNowText2 = default;
        EveNowText3 = default;
        
        EveNextText1 = default;
        EveNextText2 = default;
        EveNextText3 = default;
        
    }

    int[] RanNowProcToRanNow = { 0, 1, 2, 3, 0, 1, 4, 5, 0, 1, 2, 0, 6, 4, 5 };
    int[] RanNowProcToEveNow = { 0, 1, 2, 3, 0, 1, 3, 4, 0, 1, 2, 0, 1, 5, 4 };

    RanNow = RanNowProcToRanNow[RanNowProc];
    EveNow = RanNowProcToEveNow[RanNowProc];

    RanNextProc = (RanNowProc + 14) % 15;
    RanNext = RanNowProcToRanNow[RanNextProc];
    EveNext = RanNowProcToEveNow[RanNextProc];

}

async Task shortHelpMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Зчитує текст для повідомлення
    string shortHelpText = langLocal.Localization.shortMenu;

    //Зчитує текст для кнопки
    string menuText = langLocal.Localization.Menu;

    //Будуємо кнопку "Меню" для повідомлення
    var inlineKeyboardMenu = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(menuText) }
    });

    //Надсилаємо повідомлення
    await botClient.SendTextMessageAsync(
        chatId: update.Message.Chat.Id,
        text: shortHelpText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyMarkup: inlineKeyboardMenu,
        replyToMessageId: update.Message.MessageId,
        cancellationToken: cancellationToken
    );
    return;
}

async Task helpMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Зчитується текст для повідомлення    
    string helpText = langLocal.Localization.HelpMessText;

    //Надсилаємо повідомлення
    await botClient.SendTextMessageAsync(
        chatId: update.Message.Chat.Id,
        text: helpText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyToMessageId: update.Message.MessageId,
        cancellationToken: cancellationToken
    );
    return;
}

async Task greatingMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken, string language)
{
    //Зчитується текст для повідомлення
    string greetingText = langLocal.Localization.Greeting;    

    //Зчитується текст для кнопки
    string menuText = langLocal.Localization.Menu;
    
    //Будуємо кнопку "Меню" для повідомлення
    var inlineKeyboardMenu = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(menuText) }
    });

    //Надсилаємо повідомлення
    await botClient.SendTextMessageAsync(
        chatId: update.Message.Chat.Id,
        text: greetingText,
        disableNotification: true,
        parseMode: ParseMode.Html,
        replyMarkup: inlineKeyboardMenu,
        replyToMessageId: update.Message.MessageId,
        cancellationToken: cancellationToken
        );
    return;
}

Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

