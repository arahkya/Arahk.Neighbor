using Arahk.Neighbor.Api.Models;
using Arahk.Neighbor.Application.Commmand.House;
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

    [HttpPost]
    public async Task<IActionResult> AddHouseAsync(AddHouseModel model)
    {
        await mediator.Send(new CreateRequest { AddressName = model.AddressName });

        return Ok();
    }
}