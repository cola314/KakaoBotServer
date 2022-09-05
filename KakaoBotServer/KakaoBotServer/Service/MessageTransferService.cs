using System.Text.Json;
using System.Threading.Channels;
using KakaoBotServer.Model;
using StackExchange.Redis;

namespace KakaoBotServer.Service;

public class MessageTransferService
{
    private readonly ILogger<MessageTransferService> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly string _pushChannel = "push_channel";
    private readonly string _messageQueueChannel = "message_queue";

    public MessageTransferService(IConnectionMultiplexer redis, ILogger<MessageTransferService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async IAsyncEnumerable<PushMessage> GetPushMessages(CancellationToken ctx)
    {
        var channel = Channel.CreateBounded<PushMessage>(Int32.MaxValue);
        var writer = channel.Writer;

        async void RedisHandler(RedisChannel channel, RedisValue value)
        {
            var message = JsonSerializer.Deserialize<PushMessage>(value.ToString());

            _logger.LogInformation("[GetPushMessages] room: {0}**, message size: {2}",
                message.Room.FirstOrDefault(), message.Message.Length);

            try
            {
                await writer.WriteAsync(message, ctx);
            }
            catch (OperationCanceledException) 
            {
                // Successfully canceled
            }
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
        _logger.LogInformation("[SendReceivedMessage] room: {0}**, sender: {1}**, message size: {2}, group chat: {3}",
            message.Room.FirstOrDefault(), message.Sender.FirstOrDefault(), message.Message.Length, message.IsGroupChat);

        var value = new RedisValue(JsonSerializer.Serialize(message));
        return _redis.GetDatabase().ListLeftPushAsync(_messageQueueChannel, value);
    }
}