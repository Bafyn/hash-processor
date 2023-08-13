using HashProcessor.RabbitMQ.Configuration;
using HashProcessor.Redis.Configuration;
using HashProcessor.Worker.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace HashProcessor.Worker;

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
            ClientProvidedName = configuration.GetValue<string>(Common.Constants.ServiceName),
            DispatchConsumersAsync = true
        };

        var connection = connectionFactory.CreateConnection();

        RabbitMQ.ConnectionHelper.DeclareRabbitMQEntities(connection);

        services.AddSingleton(connection);

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HashProcessorConnectionString");

        services.AddDbContextFactory<HashProcessorDbContext>(builder =>
        {
            // NOTE: As the number of records in the DB might be huge,
            // it makes sense to apply partitioning (sharding) strategy to distribute data rows across multiple instances of the DB
            // (as the initial step splitting into multiple tables in the same DB instance might be acceptable).
            builder.UseSqlServer(connectionString);
            builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return services;
    }
}
