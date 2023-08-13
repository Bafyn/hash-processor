using HashProcessor.Domain.Models;

namespace HashProcessor.Application.Requests.GetHashes;

public class GetHashesRequestResponse
{
    public HashSummary[] Hashes { get; init; }
}
