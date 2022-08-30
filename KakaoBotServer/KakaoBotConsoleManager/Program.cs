using System.Text.Json;
using KakaoBotConsoleManager.Model;
using StackExchange.Redis;

var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false,ConnectTimeout=3000");

while (true)
{
    try
    {
        var value = await redis.GetDatabase().ExecuteAsync("BRPOP", "message_queue", 4);
        if (!value.IsNull)
        {
            var result = value.ToDictionary().First().Value?.ToString();
            var message = JsonSerializer.Deserialize<ReceivedMessage>(result);
            var pushMessage = new PushMessage()
            {
                Message = message.Message,
                Room = message.Room,
            };
            var echoResponse = JsonSerializer.Serialize(pushMessage);

            Console.WriteLine("[Echo Message] " + echoResponse);
            await redis.GetSubscriber().PublishAsync("push_channel", echoResponse);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[Redis Error] " + ex.Message);
    }
}