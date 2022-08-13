using System.Text.Json.Serialization;

namespace KakaoBotConsoleManager.Model;

public class PushMessage
{
    [JsonPropertyName("room")]
    public string Room { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }
}