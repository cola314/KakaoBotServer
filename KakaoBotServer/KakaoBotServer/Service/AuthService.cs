using KakaoBotServer.Config;

namespace KakaoBotServer.Service;

public class AuthService
{
    private readonly EnvironmentConfig _environmentConfig;

    public AuthService(EnvironmentConfig environmentConfig)
    {
        _environmentConfig = environmentConfig;
    }

    public bool IsValidApiKey(string apiKey)
    {
        return _environmentConfig.API_KEY == apiKey;
    }
}