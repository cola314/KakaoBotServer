using System.Text.Json;
using KakaoBotConsoleManager.Model;
using StackExchange.Redis;

var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false,ConnectTimeout=3000");

while (true)
{
    try
    {
        var value = redis.GetDatabase().ListLeftPop("message_queue");
        if (value.IsNull)
        {
            await Task.Delay(1);
        }
        else
        {
            var message = JsonSerializer.Deserialize<ReceivedMessage>(value.ToString());
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