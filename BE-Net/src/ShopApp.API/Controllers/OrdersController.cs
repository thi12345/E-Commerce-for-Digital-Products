using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.Orders.Commands.PlaceOrder;
using ShopApp.Application.Orders.Queries.GetOrderById;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Place([FromBody] PlaceOrderCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
