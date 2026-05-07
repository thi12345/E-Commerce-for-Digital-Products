using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.Users.Commands.CreateUser;
using ShopApp.Application.Users.Commands.DeleteUser;
using ShopApp.Application.Users.Commands.UpdateUser;
using ShopApp.Application.Users.Queries.GetUserById;
using ShopApp.Application.Users.Queries.GetUsers;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetUsersQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateUserCommand(id, request.Name, request.Email, request.Role), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteUserCommand(id), ct);
        return NoContent();
    }
}

public record UpdateUserRequest(string Name, string Email, string Role);
