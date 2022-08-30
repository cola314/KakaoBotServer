using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using GrpcProto;

var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});
var channel = GrpcChannel.ForAddress("https://localhost:7282", new GrpcChannelOptions()
{
    HttpHandler = httpHandler
});

var client = new KakaoClient.KakaoClientClient(channel);

var apiKey = "ApiKey";

_ = Task.Run(async () =>
{
    while (true)
    {
        try
        {
            Console.WriteLine("[ReadPushMessage]");
            using var call = client.ReadPushMessage(new ReadPushMessageRequest() {ApiKey = apiKey});
            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                var pushMessage = call.ResponseStream.Current;
                Console.WriteLine($"[Push Message] Room: {pushMessage.Room}, Message: {pushMessage.Message}");
            }
        }
        catch (RpcException ex)
        {
            if (ex.StatusCode == StatusCode.Unauthenticated)
            {
                Console.WriteLine("[Unauthenticated]" + ex.Message);
                break;
            }
            Console.WriteLine("[ReadPushMessage RpcException]" + ex.Message);

            await Task.Delay(1000);
        }
    }
});

while (true)
{
    try
    {
        var line = Console.ReadLine();
        Console.WriteLine($"[SendReceivedMessageAsync] {line}");
        await client.SendReceivedMessageAsync(new SendReceivedMessageRequest()
        {
            ApiKey = apiKey,
            Sender = "ConsoleClient",
            Room = "ConsoleClientRoom",
            IsGroupChat = false,
            Message = line,
        });
    }
    catch (RpcException ex)
    {
        if (ex.StatusCode == StatusCode.Unauthenticated)
        {
            Console.WriteLine("[Unauthenticated]" + ex.Message);   
            break;
        }
        Console.WriteLine("[SendReceivedMessageAsync RpcException]" + ex.Message);
    }
}