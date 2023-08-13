using EFCore.BulkExtensions;
using HashProcessor.Domain.Entities;
using HashProcessor.Domain.Models;
using HashProcessor.Worker.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace HashProcessor.Worker.Services;

internal class RabbitMQConsumer
{
    // For future uses it's advisable to hide the DAL implementation behind a custom service 
    // instead of exposing the DbContext to the external clients.
    private readonly IDbContextFactory<HashProcessorDbContext> _dbContextFactory;
    private readonly IModel _channel;
    private readonly IDatabase _redis;

    public RabbitMQConsumer(IDbContextFactory<HashProcessorDbContext> dbContextFactory, IConnection rabbitMQConnection, IDatabase redis)
    {
        _dbContextFactory = dbContextFactory;
        _channel = rabbitMQConnection.CreateModel();
        _redis = redis;
    }

    public void Consume()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        // This might result in more than 4 threads processing data as the method is async and we do not capture the synchronization context.
        // But the number of threads (hence active RabbitMQ channels) is 4 so that messages are consumed in parallel across 4 instances.
        consumer.Received += ConsumeCoreAsync;

        _channel.BasicConsume(queue: RabbitMQ.Constants.HashQueueName, autoAck: true, consumer: consumer);
    }

    private async Task ConsumeCoreAsync(object model, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var hashDto = JsonSerializer.Deserialize<HashDto>(message);

        var hashes = hashDto.Hashes.Select(h => new Hash()
        {
            Date = hashDto.Date,
            Value = Convert.ToHexString(h)
        }).ToList();

        await using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            await dbContext.BulkInsertAsync(hashes);
        };

        await _redis.HashIncrementAsync(Redis.Constants.HashSummaryKey, Redis.KeyHelper.GetDateKey(hashDto.Date), hashes.Count);

        Console.WriteLine($"Processed {hashes.Count} hashes in thread {Environment.CurrentManagedThreadId}");
    }
}
