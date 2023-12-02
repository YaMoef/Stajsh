using Application.CQRS.Template;
using Infrastructure.Services.FetchBinaryService;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BinaryController: ApiV1Controller
{
    private readonly IMediator _mediator;
    private readonly IFetchBinaryService _fetchBinaryService;
    public BinaryController(IMediator mediator, IFetchBinaryService fetchBinaryService)
    {
        _mediator = mediator;
        _fetchBinaryService = fetchBinaryService;
    }

    [HttpGet]
    [Route("/dist/{version}/{flavor}")]
    public async Task<IActionResult> GetAll(string version, string flavor)
    {
        await _fetchBinaryService.FetchNodeBinary(version, flavor);
        return Ok();
    }
}