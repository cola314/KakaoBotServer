using System.Text.Json.Serialization;

namespace KakaoBotServer.Model;

public class PushMessage
{
    [JsonPropertyName("room")]
    public string Room { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }
}