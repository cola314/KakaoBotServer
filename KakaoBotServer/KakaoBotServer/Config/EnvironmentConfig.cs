namespace KakaoBotServer.Config;

public class EnvironmentConfig
{
    public string REDIS_SERVER { get; }
    public string REDIS_PORT { get; }
    public string API_KEY { get; }

    public EnvironmentConfig(ILogger<EnvironmentConfig> logger)
    {
        try
        {
            logger.LogInformation("[Load environment variables]");

            REDIS_SERVER = GetStringEnvironmentOrThrow(nameof(REDIS_SERVER));
            REDIS_PORT = GetStringEnvironmentOrThrow(nameof(REDIS_PORT));
            API_KEY = GetStringEnvironmentOrThrow(nameof(API_KEY));
        }
        catch (Exception ex)
        {
            logger.LogError("[Fail to Load environment variables] {0}", ex.Message);
            Environment.Exit(1);
        }
    }
    private string GetStringEnvironmentOrThrow(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? throw new ArgumentException($"No such environment variable {key}");
    }
}