using Grpc.Core;
using GrpcProto;
using KakaoBotServer.Model;
using KakaoBotServer.Service;
using StackExchange.Redis;

namespace KakaoBotServer.GrpcServices;

public class KakaoClientService : KakaoClient.KakaoClientBase
{
    private readonly ILogger _logger;
    private readonly AuthService _authService;
    private readonly MessageTransferService _messageTransferService;

    public KakaoClientService(ILogger<KakaoClientService> logger, AuthService authService, MessageTransferService messageTransferService)
    {
        _logger = logger;
        _authService = authService;
        _messageTransferService = messageTransferService;
    }

    public override async Task ReadPushMessage(ReadPushMessageRequest request, IServerStreamWriter<PushMessageResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("[ReadPushMessage] {0}", context.Peer);
        ThrowIfApiKeyIsInvalid(request.ApiKey);

        await foreach (var message in _messageTransferService.GetPushMessages(context.CancellationToken))
        {
            var response = new PushMessageResponse()
            {
                Message = message.Message,
                Room = message.Room,
            };
            await responseStream.WriteAsync(response);
        }
    }

    public override async Task<SendReceivedMessageResponse> SendReceivedMessage(SendReceivedMessageRequest request, ServerCallContext context)
    {
        ThrowIfApiKeyIsInvalid(request.ApiKey);

        var message = new ReceivedMessage()
        {
            Sender = request.Sender,
            IsGroupChat = request.IsGroupChat,
            Message = request.Message,
            Room = request.Room,
        };
        await _messageTransferService.SendReceivedMessage(message);

        return new SendReceivedMessageResponse();
    }

    private void ThrowIfApiKeyIsInvalid(string apiKey)
    {
        if (!_authService.IsValidApiKey(apiKey))
        {
            var metadata = new Metadata()
            {
                {"ApiKey", apiKey }
            };
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid ApiKey"), metadata);
        }
    }
}