using KakaoBotServer.Config;
using KakaoBotServer.GrpcServices;
using KakaoBotServer.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<EnvironmentConfig>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<MessageTransferService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetService<EnvironmentConfig>();
    var connectionString = $"{config.REDIS_SERVER}:{config.REDIS_PORT},allowAdmin=true,ConnectTimeout=3000";
    return ConnectionMultiplexer.Connect(connectionString);
});

var app = builder.Build();

app.MapGrpcService<KakaoClientService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");

app.Run();
