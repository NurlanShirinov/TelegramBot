﻿using Newtonsoft.Json;

namespace TelegramBotA;

public class ProjectInitializer
{
    public static Config Config { get; set; }

    public static void InitializeConfig()
    {
        using var reader = new StreamReader(@"C:\Users\My_Vivobook\source\repos\MyCourses\nArchitureDemo\src\corePackages\TelegramBotA\TelegramBotA\Config.json");
        if (reader != null)
        {
            var configFile = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());

            Config = new Config();

            if (configFile != null)
            {
                Config.CommandPrefix = configFile.CommandPrefix;
                Config.API_KEY = configFile.API_KEY;
                Config.Bot_Status = configFile.Bot_Status;
                Config.Chat_Id = configFile.Chat_Id;
                Config.Available_Commands = configFile.AvailableCommandsText!.Split(',').ToList();
            }
        }
    }
}
