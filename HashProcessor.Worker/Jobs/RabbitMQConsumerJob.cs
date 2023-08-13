using HashProcessor.Worker.Persistence.Context;
using HashProcessor.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace HashProcessor.Worker.Jobs;

internal class RabbitMQConsumerJob : BackgroundService
{
    private readonly Thread[] _threads;
    private readonly Func<RabbitMQConsumer> _consumerFactory;

    public RabbitMQConsumerJob(
        IDbContextFactory<HashProcessorDbContext> dbContextFactory,
        IConnection rabbitMQConnection,
        IConnectionMultiplexer redisConnection)
    {
        // TODO: Consumer implementation should be hidden and exposed via an abstraction and DI.
        _consumerFactory = () => new RabbitMQConsumer(dbContextFactory, rabbitMQConnection, redisConnection.GetDatabase());
        _threads = new Thread[Constants.MaxDOP];
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (int i = 0; i < _threads.Length; i++)
        {
            var thread = new Thread(ThreadInstanceUnitOfWork)
            {
                IsBackground = true
            };

            _threads[i] = thread;
        }

        Array.ForEach(_threads, t => t.Start());

        return Task.CompletedTask;
    }

    private void ThreadInstanceUnitOfWork()
    {
        Console.WriteLine($"Running thread {Environment.CurrentManagedThreadId}");

        var consumer = _consumerFactory();
        consumer.Consume();
    }
}
