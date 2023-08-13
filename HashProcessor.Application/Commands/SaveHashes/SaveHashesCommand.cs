using MediatR;

namespace HashProcessor.Application.Commands.SaveHashes;

public class SaveHashesCommand : IRequest
{
    public int Count { get; set; }
}
