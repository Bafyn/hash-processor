using HashProcessor.Domain.Models;
using HashProcessor.Redis;
using MediatR;
using StackExchange.Redis;

namespace HashProcessor.Application.Requests.GetHashes;

public class GetHashesRequestHandler : IRequestHandler<GetHashesRequest, GetHashesRequestResponse>
{
    private readonly IDatabase _redis;

    public GetHashesRequestHandler(IConnectionMultiplexer redisConnection)
    {
        _redis = redisConnection.GetDatabase();
    }

    public async Task<GetHashesRequestResponse> Handle(GetHashesRequest request, CancellationToken cancellationToken)
    {
        var hashSummaryEntries = await _redis.HashGetAllAsync(Redis.Constants.HashSummaryKey);

        var hashSummaries = hashSummaryEntries.Select(e => new HashSummary()
        {
            Date = KeyHelper.ParseDateKey(e.Name),
            Count = (long)e.Value
        });

        return new GetHashesRequestResponse()
        {
            Hashes = hashSummaries.ToArray()
        };
    }
}
