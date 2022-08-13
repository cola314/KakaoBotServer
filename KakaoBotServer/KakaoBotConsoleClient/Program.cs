using Grpc.Core;
using Grpc.Net.Client;
using GrpcProto;

var channel = GrpcChannel.ForAddress("https://localhost:7282");

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

            await Task.Delay(1000);
        }
    }
});

while (true)
{
    try
    {
        var line = Console.ReadLine();
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
        Console.WriteLine("[SendReceivedMessageAsync RpcException]" + ex.Message);
    }
}