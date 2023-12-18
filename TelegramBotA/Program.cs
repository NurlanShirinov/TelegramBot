using TelegramBotA;

ProjectInitializer.InitializeConfig(); // Base project Initialization done.

TelegramBot telegramBot = new();
telegramBot.Start();

Console.ReadKey();
