using HashProcessor.RabbitMQ.Configuration;
using HashProcessor.Redis.Configuration;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace HashProcessor.API;

internal static class ServiceCollectionSetup
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>();
        var configurationOptions = new ConfigurationOptions()
        {
            ClientName = configuration.GetValue<string>(Common.Constants.ServiceName),
            DefaultDatabase = redisConfiguration.DefaultDatabase
        };

        configurationOptions.EndPoints.Add(redisConfiguration.Host, redisConfiguration.Port);

        var connection = ConnectionMultiplexer.Connect(configurationOptions);

        services.AddSingleton<IConnectionMultiplexer>(connection);

        return services;
    }

    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQConfiguration = configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
        var connectionFactory = new ConnectionFactory()
        {
            HostName = rabbitMQConfiguration.Host,
            Port = rabbitMQConfiguration.Port,
            UserName = rabbitMQConfiguration.Username,
            Password = rabbitMQConfiguration.Password,
            VirtualHost = "/",
            ClientProvidedName = configuration.GetValue<string>(Common.Constants.ServiceName)
        };

        var connection = connectionFactory.CreateConnection();

        RabbitMQ.ConnectionHelper.DeclareRabbitMQEntities(connection);

        services.AddSingleton(connection);

        return services;
    }
}
