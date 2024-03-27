using MM_Helper_in_TG.Properties.Image;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
using langLocal = MM_Helper_in_TG.Properties.Languages;


var token = "your_bot_token";

// Creating botClient and CancallationTokenSource object
TelegramBotClient botClient = new TelegramBotClient(token);
using CancellationTokenSource cts = new();



// Validation dates for servers 116 and 127
DateTime CheckDate116 = new DateTime(2023, 6, 10);
DateTime CheckDate127 = new DateTime(2023, 6, 11);

// Control date for updating ratings
DateTime CheckRanking = new DateTime(2023, 6, 12, 8, 0, 0);

// Current date and time in hours and minutes
string currentTime = DateTime.Now.ToString("H:mm");

// Time to receive notifications about the start of daily events
List<string> GASTime = new List<string> { "4:00", "12:00", "20:00" };
List<string> MATime = new List<string> { "00:01", "8:00", "16:00", "24:00" };
List<string> ShopTime = new List<string> { "00:00", "2:00", "4:00", "6:00", "8:00", "10:00", "12:00", "14:00", "16:00", "18:00", "20:00", "22:00"};



// Initialization of variables for Ranks and Events
int RanNow;
int RanNext;
int EveNow;
int EveNext;

// Cells for current events
string? EveNowText1 = default;
string? EveNowText2 = default;
string? EveNowText3 = default;

// Cells for the following events
string? EveNextText1 = default;
string? EveNextText2 = default;
string? EveNextText3 = default;


// List of users for various reminders
Dictionary<ChatId, string>? eventNotifUsers = LoadUsers("eventNotifUsers"); // event reminder users
Dictionary<ChatId, string>? banquetShopUsers = LoadUsers("banquetShopUsers"); // banquet store reminder users
Dictionary<ChatId, string>? MAUsers = LoadUsers("MAUsers"); // mountain adventure reminder users
Dictionary<ChatId, string>? GASUsers = LoadUsers("GASUsers"); // users of great ape sediment reminders



// A name, description, and short description for the bot
// A name for the bot
await botClient.SetMyNameAsync(langLocal.Localization.botName, default, default); // Назва бота, англійською за замовчуванням

// Description for the bot
await botClient.SetMyDescriptionAsync(langLocal.Localization.botDescription, default, default); // Опис бота, англійською за замовчуванням

// Short description for the bot
await botClient.SetMyShortDescriptionAsync(langLocal.Localization.botShortDescription, default, default); // Короткий опис бота, англійською за замовчуванням



// Set the bot commands
// Scopes for commands are set
BotCommandScopeAllPrivateChats scoreAllPrivateChats = new BotCommandScopeAllPrivateChats(); // Всі приватні чати
BotCommandScopeAllGroupChats scoreAllGroupChats = new BotCommandScopeAllGroupChats(); // Участники всіх груп та супергруп
BotCommandScopeAllChatAdministrators scopeAllChatAdministrators = new BotCommandScopeAllChatAdministrators(); // Адміністратори всіх груп та супергруп

// List of commands for all private groups
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

// List of teams for participants of all groups and supergroups
List<BotCommand> commandsAllGroupChats = new List<BotCommand>
{
    new BotCommand { Command = "shorthelp", Description = langLocal.Localization.shortHelp },
    new BotCommand { Command = "help", Description = langLocal.Localization.help },
    new BotCommand { Command = "ratingsnow", Description = langLocal.Localization.ratingsNow },
    new BotCommand { Command = "ratingsnext", Description = langLocal.Localization.ratingsNext },
    new BotCommand { Command = "eventsnow", Description = langLocal.Localization.eventsNow },
    new BotCommand { Command = "eventsnext", Description = langLocal.Localization.eventsNext }
};

// List of commands for administrators of all groups and supergroups
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

// Set the bot commands
await botClient.SetMyCommandsAsync(commandsAllPrivateChats, scoreAllPrivateChats, default); // всі приватні чати
await botClient.SetMyCommandsAsync(commandsAllGroupChats, scoreAllGroupChats, default); // всі групи та супергрупи
await botClient.SetMyCommandsAsync(commandsAllChatAdministrators, scopeAllChatAdministrators, default); //всі адміністратори груп та супергруп



// Lists of events and control dates
// that repeat every 54 days
Dictionary<string, DateTime> events54days = new Dictionary<string, DateTime> 
{
    {langLocal.Localization.WorldCommerceText, new DateTime(2023, 5, 31, 8, 0, 0)},
    { langLocal.Localization.MooncakeText, new DateTime(2023, 6, 6, 8, 0, 0)},
    {langLocal.Localization.MonopolyCarnivalText, new DateTime(2023, 6, 12, 8, 0, 0)},
    {langLocal.Localization.WonderfulMelodyText, new DateTime(2023, 6, 18, 8, 0, 0)},
    {langLocal.Localization.IslandBattleText, new DateTime(2023, 5, 1, 8, 0, 0)},
    {langLocal.Localization.ABoxOfFruitText, new DateTime(2023, 5, 7, 8, 0, 0)},
    {langLocal.Localization.CaravanEscortText, new DateTime(2023, 5, 13, 8, 0, 0)},
    {langLocal.Localization.TombTreasureText, new DateTime(2023, 5, 19, 8, 0, 0)}    
};

// that repeat every 30 days
Dictionary<string, DateTime> events30days = new Dictionary<string, DateTime>
{
    {langLocal.Localization.BazaarTreasureText, new DateTime(2023, 5, 28, 8, 0, 0)},
    {langLocal.Localization.HolyTreeBlessingText, new DateTime(2023, 5, 22, 8, 0, 0)},
    {langLocal.Localization.MidAutumnDiceGameText, new DateTime(2023, 5, 16, 8, 0, 0)},
    {langLocal.Localization.WishdomScholarText, new DateTime(2023, 5, 10, 8, 0, 0)},
    {langLocal.Localization.CoinDivinationText, new DateTime(2023, 5, 4, 8, 0, 0)}
};

// that repeat every 48 days
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

User me = await botClient.GetMeAsync();
Console.WriteLine($"I am user {me.Id} and my name is {me.FirstName}.");

// Creating a repeating cycle
while (true)
{
    // for banquet store reminders
    if (ShopTime.Contains(currentTime))
    {
        Parallel.ForEach(banquetShopUsers.Keys, async user =>
        {
            try
            {
                string TextMessage = langLocal.Localization.TextBanquetShop;

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

    // for the great ape siege reminders
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

    // for mountain adventure reminders
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


    Thread.Sleep(60000); // To wait for 1 minute
}



async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(client, update, cancellationToken);
        return;
    }
    
    if (!(update.Message is Message message) || !(message.Text is string messageText)) return;

    if ((DateTime.UtcNow - message.Date).TotalMinutes > 1) return;
    
    if (update.Message.Text.StartsWith("/")) { await HandleComamnd(client, update, cancellationToken); }
}

async Task HandleComamnd (ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    if (!(update.Message is Message message) || !(message.Text is string messageText)) return;
    if ((DateTime.UtcNow - message.Date).TotalMinutes > 1) return;

    
    if (messageText.Contains("start")) { await greatingMessage(client, update, cancellationToken); }
    else if (messageText.Contains("shorthelp")) { await shortHelpMessage(client, update, cancellationToken); }
    else if (messageText.Contains("help")) { await helpMessage(client, update, cancellationToken); }
    else if (messageText.Contains("ratingsnow")) { await ratingsNowMessage(client, update, cancellationToken); }
    else if (messageText.Contains("ratingsnext")) { await ratingsNextMessage(client, update, cancellationToken); }
    else if (messageText.Contains("eventsnow")) { await eventsNowMessage(client, update, cancellationToken); }
    else if (messageText.Contains("eventsnext")) { await eventsNextMessage(client, update, cancellationToken); }
    else if (messageText.Contains("banqnotif")) { await BanquetShopMessage(client, update, cancellationToken); }
    else if (messageText.Contains("gasnotif")) { await GASMessage(client, update, cancellationToken); }
    else if (messageText.Contains("manotif")) { await MAMessage(client, update, cancellationToken); }
}


async Task HandleCallbackQuery(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{    
    CallbackQuery? callbackQuery = update.CallbackQuery;

    // Setting text for buttons
    string Menu = langLocal.Localization.Menu;
    
    string Rating = langLocal.Localization.Rating;    
    string RatingNow = langLocal.Localization.RatingNow;    
    string RatingNext = langLocal.Localization.RatingNext;
    
    string Event = langLocal.Localization.Event;    
    string EventNow = langLocal.Localization.EventNow;    
    string EventNext = langLocal.Localization.EventNext;    
    
    string Guild = langLocal.Localization.Guild;    
    string BanqShop = langLocal.Localization.BanqShop;    
    string GAS = langLocal.Localization.GAS;    
    string MA = langLocal.Localization.MA;    
    string ShopingRow = langLocal.Localization.ShopingRow;
    
    string HideMenu = langLocal.Localization.HideMenu;    
    string BackToMenu = langLocal.Localization.BackToMenu;


    // Creating lists with buttons
    // List of buttons when clicking on "Menu"
    InlineKeyboardMarkup inlineKeyboardMenuFull = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(Rating) },
        new [] { InlineKeyboardButton.WithCallbackData(Event) },
        new [] { InlineKeyboardButton.WithCallbackData(BanqShop) },
        new [] { InlineKeyboardButton.WithCallbackData(GAS) },
        new [] { InlineKeyboardButton.WithCallbackData(MA) },
        new [] { InlineKeyboardButton.WithCallbackData(HideMenu) }
    });

    // List of buttons when clicking on "Rating"
    InlineKeyboardMarkup inlineKeyboardRatingMenu = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(RatingNow) },
        new [] { InlineKeyboardButton.WithCallbackData(RatingNext) },
        new [] { InlineKeyboardButton.WithCallbackData(BackToMenu) }
    });

    // List of buttons when clicking on "Events"
    InlineKeyboardMarkup inlineKeyboardEventsMenu = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(EventNow) },
        new [] { InlineKeyboardButton.WithCallbackData(EventNext) },
        new [] { InlineKeyboardButton.WithCallbackData(BackToMenu) }
    });

    // Menu button
    InlineKeyboardMarkup inlineKeyboardMenu = new InlineKeyboardMarkup(new[]
    {
         new [] { InlineKeyboardButton.WithCallbackData(Menu) },
    });

    

    if (callbackQuery.Data.Equals(Menu))
    {             
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
        await ratingsNowMessage(client, update, cancellationToken);
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: cancellationToken);
        return;
    }

    if (callbackQuery.Data.Equals(RatingNext))
    {
        await ratingsNextMessage(client, update, cancellationToken);
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
        await eventsNowMessage(client, update, cancellationToken);
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(EventNext))
    {
        await eventsNextMessage(client, update, cancellationToken);
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(BanqShop))
    {
        await BanquetShopMessage(client, update, cancellationToken);
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(GAS))
    {
        await GASMessage(client, update, cancellationToken);
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        return;
    }

    if (callbackQuery.Data.Equals(MA))
    {
        await MAMessage(client, update, cancellationToken);
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
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



async Task MAMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Define the type of update
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    // Reads the text for the message
    string MAStartText = langLocal.Localization.MAStart;
    string MAStopText = langLocal.Localization.MAStop;

    string messageText = null;

    //Defining the text of the message
    if (!MAUsers.ContainsKey(chatId))
    {        
        messageText = MAStartText;
        MAUsers.Add(chatId, selectLanguage(client, update, cancellationToken));
    }
    else
    {
        messageText = MAStopText;
        MAUsers.Remove(chatId);
    }

    // Saving the user list
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

async Task GASMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Define the type of update
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    // Reads the text for the message
    string GASStartText = langLocal.Localization.GASStart;
    string GASStopText = langLocal.Localization.GASStop;

    string messageText = null;

    //Defining the text of the message
    if (!GASUsers.ContainsKey(chatId))
    {
        messageText = GASStartText;
        GASUsers.Add(chatId, selectLanguage(client, update, cancellationToken));
    }
    else
    {
        messageText = GASStopText;
        GASUsers.Remove(chatId);
    }

    // Saving the user list
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

async Task BanquetShopMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Define the type of update
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    // Reads the text for the message
    string BanquetShopStartText = langLocal.Localization.BanqStart;
    string BanquetShopStopText = langLocal.Localization.BanqStop;

    string messageText = null;

    //Defining the text of the message
    if (!banquetShopUsers.ContainsKey(chatId))
    {
        messageText = BanquetShopStartText;
        banquetShopUsers.Add(chatId, selectLanguage(client, update, cancellationToken));
    }
    else
    {
        messageText = BanquetShopStopText;
        banquetShopUsers.Remove(chatId);
    }

    // Saving the user list
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

void SaveUsers(Dictionary<ChatId, string> dictionary, string nameFile)
{
    string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
    string filePath = Path.Combine(basePath, "Users", $"{nameFile}.json");

    try
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Attempting to create a directory
        string json = JsonConvert.SerializeObject(dictionary);
        File.WriteAllText(filePath, json);
    }
    catch (Exception ex)
    {
        // Handling possible errors when creating a directory or writing to a file
        Console.WriteLine($"Error writing to file: {ex.Message}");
    }
}

Dictionary<ChatId, string>? LoadUsers(string nameFile)
{
    // Get the base path for user files:
    string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
    // Combine with the "Users" subdirectory and the specified filename:
    string filePath = Path.Combine(basePath, "Users", $"{nameFile}.json");

    // Check if the file exists:
    if (File.Exists(filePath))
    {
        // Read the JSON content from the file:
        string json = File.ReadAllText(filePath);
        // Deserialize the JSON into a dictionary of ChatId to string:
        return JsonConvert.DeserializeObject<Dictionary<ChatId, string>>(json);
    }
    else
    {
        // File doesn't exist, return an empty dictionary:
        return new Dictionary<ChatId, string>();
    }
}

async Task eventsNextMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Determine the type of update
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    // Call the method for calculating 3 days and events
    counterThreeDays();
    checkingEvents();

    string[] ENextText = new string[4];
    string[] imagenEveNext = new string[4];

    // Read the text and image for the message
    string EventNext = langLocal.Localization.eventNextMess;
    Stream imageStream = null;
    InputFile imagenEveNext0 = null;

    switch (EveNext)
    {
        case 0:
            ENextText[0] = langLocal.Localization.eventTextNext0;
            imageStream = new MemoryStream(Images.eventImage0);
            imagenEveNext0 = InputFile.FromStream(imageStream, "eventImage0");
            break;
        case 1:
            ENextText[0] = langLocal.Localization.eventTextNext1;
            imageStream = new MemoryStream(Images.eventImage1);
            imagenEveNext0 = InputFile.FromStream(imageStream, "eventImage1");
            break;
        case 2:
            ENextText[0] = langLocal.Localization.eventTextNext2;
            imageStream = new MemoryStream(Images.eventImage2);
            imagenEveNext0 = InputFile.FromStream(imageStream, "eventImage2");
            break;
        case 3:
            ENextText[0] = langLocal.Localization.eventTextNext3;
            imageStream = new MemoryStream(Images.eventImage3);
            imagenEveNext0 = InputFile.FromStream(imageStream, "eventImage3");
            break;
        case 4:
            ENextText[0] = langLocal.Localization.eventTextNext4;
            imageStream = new MemoryStream(Images.eventImage4);
            imagenEveNext0 = InputFile.FromStream(imageStream, "eventImage4");
            break;
        case 5:
            ENextText[0] = langLocal.Localization.eventTextNext5;
            imageStream = new MemoryStream(Images.eventImage5);
            imagenEveNext0 = InputFile.FromStream(imageStream, "eventImage5");
            break;
    }


    if (EveNextText1 != null || EveNextText1 != default) { ENextText[1] = EveNextText1;}
    if (EveNextText2 != null || EveNextText1 != default) { ENextText[2] = EveNextText2;}
    if (EveNextText3 != null || EveNextText1 != default) { ENextText[3] = EveNextText3;}

    // Sending the message
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

async Task eventsNowMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Determine the type of update
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    // Call the method for calculating 3 days and events
    counterThreeDays();
    checkingEvents();

    string[] ENowText = new string[4];

    // Read the text and image for the message
    string EventNow = langLocal.Localization.eventNowMess;
    Stream imageStream = null;
    InputFile imagenEveNow0 = null;

    switch (EveNow)
    {
        case 0:
            ENowText[0] = langLocal.Localization.eventTextNow0;
            imageStream = new MemoryStream(Images.eventImage0);
            imagenEveNow0 = InputFile.FromStream(imageStream, "eventImage0");
            break;
        case 1:
            ENowText[0] = langLocal.Localization.eventTextNow1;
            imageStream = new MemoryStream(Images.eventImage1);
            imagenEveNow0 = InputFile.FromStream(imageStream, "eventImage1");
            break;
        case 2:
            ENowText[0] = langLocal.Localization.eventTextNow2;
            imageStream = new MemoryStream(Images.eventImage2);
            imagenEveNow0 = InputFile.FromStream(imageStream, "eventImage2");
            break;
        case 3:
            ENowText[0] = langLocal.Localization.eventTextNow3;
            imageStream = new MemoryStream(Images.eventImage3);
            imagenEveNow0 = InputFile.FromStream(imageStream, "eventImage3");
            break;
        case 4:
            ENowText[0] = langLocal.Localization.eventTextNow4;
            imageStream = new MemoryStream(Images.eventImage4);
            imagenEveNow0 = InputFile.FromStream(imageStream, "eventImage4");
            break;
        case 5:
            ENowText[0] = langLocal.Localization.eventTextNow5;
            imageStream = new MemoryStream(Images.eventImage5);
            imagenEveNow0 = InputFile.FromStream(imageStream, "eventImage5");
            break;
    }


    if (EveNowText1 != null || EveNowText1 != default) { ENowText[1] = EveNowText1; }
    if (EveNowText2 != null || EveNowText2 != default) { ENowText[2] = EveNowText2; }
    if (EveNowText3 != null || EveNowText3 != default) { ENowText[3] = EveNowText3; }

    // Sending the message
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

void checkingEvents()
{

    // **Handling events recurring every 54 days**
    foreach (var eventName in events54days.Keys.ToList())
    {
        DateTime eventDateTime = events54days[eventName];
        // Loop until the eventDateTime is past the current date
        while (eventDateTime.AddDays(54) <= DateTime.Now)
        {
            // Update eventDateTime to the next occurrence (every 54 days)
            eventDateTime = eventDateTime.AddDays(54);
            // Update the eventDateTime in the events54days dictionary
            events54days[eventName] = eventDateTime;
        }
        // Check if the event falls within the "Now" window (today and next 3 days)
        if (eventDateTime <= DateTime.Now && eventDateTime.AddDays(3) >= DateTime.Now)
        {
            String PathFile = eventName;
            // Assign the event name to EveNowText variables based on availability
            if (EveNowText1 == null) { EveNowText1 = PathFile; }
            else if (EveNowText2 == null) { EveNowText2 = PathFile; }
        }
        // Check if the event falls within the "Next" window (3 days from now to 6 days from now)
        if (eventDateTime.AddDays(54) <= DateTime.Now.AddDays(3) && eventDateTime.AddDays(57) >= DateTime.Now.AddDays(3))
        {
            String PathFile = eventName;
            // Assign the event name to EveNextText variables based on availability
            if (EveNextText1 == null) { EveNextText1 = PathFile; }
            else if (EveNextText2 == null) { EveNextText2 = PathFile; }
        }
    }
    // **Handling events recurring every 30 days**
    foreach (var eventName in events30days.Keys.ToList())
    {
        DateTime eventDateTime = events30days[eventName];
        // Loop until the eventDateTime is past the current date
        while (eventDateTime.AddDays(30) <= DateTime.Now)
        {
            // Update eventDateTime to the next occurrence (every 30 days)
            eventDateTime = eventDateTime.AddDays(30);
            // Update the eventDateTime in the events30days dictionary
            events30days[eventName] = eventDateTime;
        }
        // Check if the event falls within the "Now" window (today and next 3 days)
        if (eventDateTime <= DateTime.Now && eventDateTime.AddDays(3) >= DateTime.Now)
        {
            String PathFile = eventName;
            // Assign the event name to EveNowText variables based on availability
            if (EveNowText1 == null) { EveNowText1 = PathFile; }
            else if (EveNowText2 == null) { EveNowText2 = PathFile; }
            else if (EveNowText3 == null) { EveNowText3 = PathFile; }            
        }
        // Check if the event falls within the "Next" window (3 days from now to 6 days from now)
        if (eventDateTime.AddDays(30) <= DateTime.Now.AddDays(3) && eventDateTime.AddDays(33) >= DateTime.Now.AddDays(3))
        {
            String PathFile = eventName;
            // Assign the event name to EveNextText variables based on availability
            if (EveNextText1 == null) { EveNextText1 = PathFile; }
            else if (EveNextText2 == null) { EveNextText2 = PathFile; }            
        }
    }
    // **Handling events recurring every 30 days**
    foreach (var eventName in events48days.Keys.ToList())
    {
        DateTime eventDateTime = events48days[eventName];
        // Loop until the eventDateTime is past the current date
        while (eventDateTime.AddDays(48) <= DateTime.Now)
        {
            // Update eventDateTime to the next occurrence (every 30 days)
            eventDateTime = eventDateTime.AddDays(48);
            // Update the eventDateTime in the events30days dictionary
            events48days[eventName] = eventDateTime;
        }
        if (eventDateTime <= DateTime.Now && eventDateTime.AddDays(3) >= DateTime.Now)
        {
            String PathFile = eventName;
            // Assign the event name to EveNowText variables based on availability
            if (EveNowText1 == null) { EveNowText1 = PathFile; }
            else if (EveNowText2 == null) { EveNowText2 = PathFile; }            
        }
        if (eventDateTime.AddDays(48) <= DateTime.Now.AddDays(3) && eventDateTime.AddDays(51) >= DateTime.Now.AddDays(3))
        {
            String PathFile = eventName;
            // Assign the event name to EveNextText variables based on availability
            if (EveNextText1 == null) { EveNextText1 = PathFile; }
            else if (EveNextText2 == null) { EveNextText2 = PathFile; }            
        }
    }
}

async Task ratingsNextMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Determine the type of update
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    // Call the method for calculating 3 days and events
    counterThreeDays();

    // Read the text and image for the message
    string ratingsNextText = null;
    switch (RanNext)
    {
        case 0:
            ratingsNextText = langLocal.Localization.textRankingNext0;
            break;
        case 1:
            ratingsNextText = langLocal.Localization.textRankingNext1;
            break;
        case 2:
            ratingsNextText = langLocal.Localization.textRankingNext2;
            break;
        case 3:
            ratingsNextText = langLocal.Localization.textRankingNext3;
            break;
        case 4:
            ratingsNextText = langLocal.Localization.textRankingNext4;
            break;
        case 5:
            ratingsNextText = langLocal.Localization.textRankingNext5;
            break;
        case 6:
            ratingsNextText = langLocal.Localization.textRankingNext6;
            break;
    }

    // Sending the message
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

async Task ratingsNowMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Determine the type of update
    ChatId chatId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
    int messageId = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.MessageId : update.Message.MessageId;

    // Call the method for calculating 3 days and events
    counterThreeDays();

    // Read the text and image for the message
    string ratingsNowText = null;
    switch (RanNow)
    {
        case 0:
            ratingsNowText = langLocal.Localization.textRankingNow0;
            break;
        case 1:
            ratingsNowText = langLocal.Localization.textRankingNow1;
            break;
        case 2:
            ratingsNowText = langLocal.Localization.textRankingNow2;
            break;
        case 3:
            ratingsNowText = langLocal.Localization.textRankingNow3;
            break;
        case 4:
            ratingsNowText = langLocal.Localization.textRankingNow4;
            break;
        case 5:
            ratingsNowText = langLocal.Localization.textRankingNow5;
            break;
        case 6:
            ratingsNowText = langLocal.Localization.textRankingNow6;
            break;
    }

    // Sending the message
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

// Function to perform calculations related to a ranking system with a three-day cycle
void counterThreeDays()
{
    // Variables to track procedure counts for current and next cycles
    int RanNowProc = 0;
    int RanNextProc = 0;

    // Loop to iterate while the next ranking check date is not in the future
    while ((CheckRanking + TimeSpan.FromDays(3)) <= DateTime.Now)
    {
        // Update the ranking check date by adding three days
        CheckRanking = CheckRanking.AddDays(3);
        // Decrement the current procedure count
        RanNowProc = RanNowProc --;
        // Handle underflow by wrapping around to the maximum value (14 in this case)
        RanNowProc = RanNowProc == -1 ? 14 : RanNowProc;
        // Reset text variables for current and next ranking periods (assuming these are used for display)
        EveNowText1 = default;
        EveNowText2 = default;
        EveNowText3 = default;
        
        EveNextText1 = default;
        EveNextText2 = default;
        EveNextText3 = default;        
    }

    // Pre-defined lookup tables for procedure and event assignments based on `RanNowProc`
    int[] RanNowProcToRanNow = { 0, 1, 2, 3, 0, 1, 4, 5, 0, 1, 2, 0, 6, 4, 5 };
    int[] RanNowProcToEveNow = { 0, 1, 2, 3, 0, 1, 3, 4, 0, 1, 2, 0, 1, 5, 4 };

    // Look up the appropriate procedure number for the current window using `RanNowProc`
    RanNow = RanNowProcToRanNow[RanNowProc];
    // Look up the appropriate event number for the current window using `RanNowProc`
    EveNow = RanNowProcToEveNow[RanNowProc];

    // Calculate the procedure number for the next three-day window
    RanNextProc = (RanNowProc + 14) % 15; // Wrap around using modulo 15
    // Look up the procedure number for the next three-day window using `RanNextProc`
    RanNext = RanNowProcToRanNow[RanNextProc];
    // Look up the event number for the next three-day window using `RanNextProc`
    EveNext = RanNowProcToEveNow[RanNextProc];

}

async Task shortHelpMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Reads the text for the message
    string shortHelpText = langLocal.Localization.shortMenu;

    // Reads the text for the button
    string menuText = langLocal.Localization.Menu;

    // Build the "Menu" button for the message
    var inlineKeyboardMenu = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(menuText) }
    });

    //Sending the message
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

async Task helpMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Reads the text for the message  
    string helpText = langLocal.Localization.HelpMessText;

    //Sending the message
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

async Task greatingMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Reads the text for the message
    string greetingText = langLocal.Localization.Greeting;

    // Reads the text for the button
    string menuText = langLocal.Localization.Menu;

    //Build the "Menu" button for the message
    var inlineKeyboardMenu = new InlineKeyboardMarkup(new[]
    {
        new [] { InlineKeyboardButton.WithCallbackData(menuText) }
    });

    //Sending the message
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

string selectLanguage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    // Check for null to avoid NullReferenceException
    if (update == null) { throw new ArgumentNullException(nameof(update), "Update cannot be null."); }

    // Try to get the LanguageCode from the message or CallbackQuery
    string language = update.Message?.From?.LanguageCode ?? update.CallbackQuery?.From?.LanguageCode;

    // Check if language is not empty or null
    if (string.IsNullOrWhiteSpace(language))
    {
        // If the LanguageCode could not be obtained, set the default to "en"
        language = "en";
    }

    return language;
}

// Asynchronous method for handling errors that occur during polling for Telegram bot updates
Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
{
  try
  {
    // Format the error message based on the exception type
    var errorMessage = exception switch
    {
      ApiRequestException apiRequestException =>
      // Handle errors specifically related to Telegram API requests
      $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
      _ =>
      // Handle any other type of exception
      exception.ToString()
    };

    // Log the error message to the console for debugging and monitoring
    Console.WriteLine(errorMessage);
  }
  catch (Exception ex)
  {
    // Handle any unexpected errors that occur within this error handling method
    Console.Error.WriteLine("Unexpected error in HandlePollingErrorAsync: " + ex.Message);
  }
  // Return a completed task representing the completion of this error handling method
  return Task.CompletedTask;
}

