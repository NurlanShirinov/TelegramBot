using ResourceHandler.Resources.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotA;

public class TelegramBot
{
    private static TelegramBotClient? botClient;

    public void Start()
    {
        botClient = new TelegramBotClient("6638668371:AAGjChQ0TPzBYg5ete7i_-7--SPDbyD7cU4");

        CancellationTokenSource cts;
        cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new UpdateType[]
           {
                    UpdateType.Message
           }
        };

        botClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cancellationToken: cts.Token);

        Console.WriteLine("Bot is running. Press any key to exit.");
    }

    private async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken token)
    {

        if (update.Type != UpdateType.Message)
            return;
        if (update.Message!.Type != MessageType.Text)
            return;


        long chatId = update.Message.Chat.Id;
        ProjectInitializer.Config.Chat_Id = chatId;

        if (!ProjectInitializer.Config.Bot_Status) await bot.SendTextMessageAsync(chatId, "I'm sorry, I can't help you at the moment.");
        else
        {
            CommandModel commandModel = CheckMessage(update.Message.Text);
            if (!commandModel.CommandIsAvailable) { await bot.SendTextMessageAsync(chatId, "I'm sorry, I couldn't understand you. Please type /help for my current command list."); return; }

            string? username = update.Message.Chat.Username;

            if (Enum.TryParse(commandModel.Command.ToUpper(), out Enums.Commands enumCommand) && Enum.IsDefined(typeof(Enums.Commands), enumCommand))
            {
                switch (enumCommand)
                {
                    case Enums.Commands.WEATHER:
                        {
                            if (!commandModel.CommandHasParameter) await bot.SendTextMessageAsync(chatId, "The weather is very nice today.");
                            else await bot.SendTextMessageAsync(chatId, "The weather for " + commandModel.CommandSections[2] + " is rainy today. \nDon't forget your umbrella (:");
                        }
                        break;
                    case Enums.Commands.ERROR:
                        {
                            await bot.SendTextMessageAsync(chatId + 1, "This will generate Error (:");
                        }
                        break;
                    case Enums.Commands.HELP:
                        {
                            await bot.SendTextMessageAsync(chatId, " /weather : Get Current Weather Information for your Location. " +
                                "\n /weather [City] : Get Current Weather Information for Selected City." +
                                "\n /error : This is Development Test Purposed Command. No Need To Use It." +
                                "\n /help : Get Information About The Commands.", cancellationToken: token);
                        }
                        break;
                    case Enums.Commands.HELLO:
                        {
                            await bot.SendTextMessageAsync(chatId, "Hello " + username + " !!", cancellationToken: token);
                        }
                        break;
                    case Enums.Commands.TODAY:
                        {
                            await bot.SendTextMessageAsync(chatId, "Today doesn't mean anything. The Important thing is your FUTURE !!", cancellationToken: token);

                        }
                        break;
                    default:
                        {
                            await bot.SendTextMessageAsync(chatId, "I'm sorry, I couldn't understand you. Please type /help for my current command list.", cancellationToken: token);
                        }
                        break;
                }
            }
        }
    }

    private CommandModel CheckMessage(string? commandString)
    {
        CommandModel commandModel = new CommandModel
        {
            CommandText = commandString!,
            CommandSections = PrepareCommandSections(commandString!)
        };
        commandModel.CommandIsAvailable = CheckIfCommandAvailable(commandModel.CommandSections);
        commandModel.Command = commandModel.CommandIsAvailable ? commandModel.CommandSections[1] : String.Empty;
        commandModel.CommandHasParameter = commandModel.CommandIsAvailable && commandModel.CommandSections.Length > 2;
        commandModel.CommandParameter = commandModel.CommandHasParameter ? commandModel.CommandSections[2] : String.Empty;

        return commandModel;
    }

    private static bool CheckIfCommandAvailable(string[] commandSections)
    {
        return ProjectInitializer.Config.Available_Commands!.Contains(commandSections[1]);
    }

    private string[] PrepareCommandSections(string commandString)
    {
        var returnArray = new List<string>
        {
            commandString.Substring(0, 1)
        };
        returnArray.AddRange(commandString.Substring(1).Split(' '));

        return returnArray.ToArray();
    }

    private static Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Error : \n {apiRequestException.ErrorCode} \n {apiRequestException.Message}",
            _ => exception.ToString()
        };

        var chatId = ProjectInitializer.Config.Chat_Id;
        bot.SendTextMessageAsync(chatId, errorMessage);

        return Task.CompletedTask;

    }


}