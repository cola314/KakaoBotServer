using System.Text.Json;
using System.Threading.Channels;
using KakaoBotServer.Model;
using StackExchange.Redis;

namespace KakaoBotServer.Service;

public class MessageTransferService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly string _pushChannel = "push_channel";
    private readonly string _messageQueueChannel = "message_queue";

    public MessageTransferService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async IAsyncEnumerable<PushMessage> GetPushMessages(CancellationToken ctx)
    {
        var channel = Channel.CreateBounded<PushMessage>(Int32.MaxValue);
        var writer = channel.Writer;

        async void RedisHandler(RedisChannel channel, RedisValue value)
        {
            var message = JsonSerializer.Deserialize<PushMessage>(value.ToString());
            await writer.WriteAsync(message, ctx);
        }

        var sub = _redis.GetSubscriber();
        await sub.SubscribeAsync(_pushChannel, RedisHandler);
        ctx.Register(async () =>
        {
            await sub.UnsubscribeAsync(_pushChannel, RedisHandler);
            writer.Complete();
        });

        await foreach (var item in channel.Reader.ReadAllAsync(ctx))
        {
            yield return item;
        }
    }

    public Task SendReceivedMessage(ReceivedMessage message)
    {
        var value = new RedisValue(JsonSerializer.Serialize(message));
        return _redis.GetDatabase().ListLeftPushAsync(_messageQueueChannel, value);
    }
}