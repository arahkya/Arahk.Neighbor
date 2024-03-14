using Arahk.Neighbor.Application.Query.House;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Arahk.Neighbor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HouseController : ControllerBase
{
    private readonly IMediator mediator;

    public HouseController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetHouseAsync()
    {
        var houses = await mediator.Send(new ListRequest());

        return Ok(houses);
    }
}