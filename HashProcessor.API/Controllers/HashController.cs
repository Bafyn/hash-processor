using HashProcessor.Application.Commands.SaveHashes;
using HashProcessor.Application.Requests.GetHashes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HashProcessor.API.Controllers;

[Route("api/hashes")]
[ApiController]
public class HashController : ControllerBase
{
    private readonly IMediator _mediator;

    public HashController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetHashesRequestResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHashes(CancellationToken cancellationToken)
    {
        var request = new GetHashesRequest();
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveHashes(CancellationToken cancellationToken)
    {
        var request = new SaveHashesCommand()
        {
            Count = 40000 // Hard-coded as per the requirements. Can be made dynamic in future versions.
        };
        await _mediator.Send(request, cancellationToken);

        return Ok();
    }
}
