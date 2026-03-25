namespace MessageService.Services;

public class AiSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
    public int HistoryWindowSize { get; set; } = 20;
}
