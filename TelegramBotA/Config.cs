namespace TelegramBotA;

public class Config
{
    public char CommandPrefix { get; set; }
    public string? API_KEY { get; set; }
    public bool Bot_Status { get; set; }
    public long Chat_Id { get; set; }
    public string? AvailableCommandsText { get; set; }
    public List<string>? Available_Commands { get; set; }
}
