namespace HashProcessor.Redis.Configuration;

public class RedisConfiguration
{
    public string Host { get; init; }

    public int Port { get; init; }

    public int DefaultDatabase { get; init; }

    // NOTE: In this setup an instance of Redis is running locally.
    // Consider adding extra props as user name / password in a real env.
}
