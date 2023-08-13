using HashProcessor.Application.Services;
using HashProcessor.Domain.Models;
using MediatR;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace HashProcessor.Application.Commands.SaveHashes;

public class SaveHashesCommandHandler : IRequestHandler<SaveHashesCommand>
{
    private readonly IHashGenerator _hashGenerator;

    private readonly ConcurrentBag<IModel> _channels; // Channels should be disposed after being used as might overload the connection.
    private readonly ThreadLocal<IModel> _threadLocalChannel; // Channels are not thread-safe

    public SaveHashesCommandHandler(IConnection rabbitMQConnection, IHashGenerator hashGenerator)
    {
        _hashGenerator = hashGenerator;

        _channels = new ConcurrentBag<IModel>();
        _threadLocalChannel = new ThreadLocal<IModel>(() =>
        {
            var channel = rabbitMQConnection.CreateModel();
            _channels.Add(channel);

            return channel;
        });
    }

    public Task Handle(SaveHashesCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var hashChunks = _hashGenerator.GenerateHashes(request.Count).Chunk(Constants.Hash.ChunkSize);
        var now = DateTime.UtcNow;

        try
        {
            Parallel.ForEach(hashChunks, chunk =>
            {
                Console.WriteLine($"Publishing a batch in thread {Environment.CurrentManagedThreadId}");
                PublishBatch(_threadLocalChannel.Value, chunk, now);
            });
        }
        finally
        {
            DisposeChannels();
        }

        Console.WriteLine($"Request completed in {stopwatch.ElapsedMilliseconds}ms");
        return Task.CompletedTask;
    }

    private static void PublishBatch(IModel channel, byte[][] chunk, DateTime now)
    {
        var hashDto = new HashDto()
        {
            Date = now,
            Hashes = chunk
        };
        var hashDtoSerialized = JsonSerializer.Serialize(hashDto);
        var hashDtoBytes = Encoding.UTF8.GetBytes(hashDtoSerialized);

        channel.BasicPublish(RabbitMQ.Constants.HashExchangeName, RabbitMQ.Constants.HashQueueName, null, (ReadOnlyMemory<byte>)hashDtoBytes);
    }

    private void DisposeChannels()
    {
        foreach (var channel in _channels)
        {
            channel.Dispose();
        }
    }
}
