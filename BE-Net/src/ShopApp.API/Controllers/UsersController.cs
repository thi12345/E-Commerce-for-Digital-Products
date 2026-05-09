using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.API.Auth;
using ShopApp.Application.Users.Commands.CreateUser;
using ShopApp.Application.Users.Commands.DeleteUser;
using ShopApp.Application.Users.Commands.UpdateUser;
using ShopApp.Application.Users.Queries.GetUserById;
using ShopApp.Application.Users.Queries.GetUsers;
using ShopApp.Domain.Users.Enums;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? email,
        [FromQuery] UserRole? role,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetUsersQuery(email, role, isActive, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        if (!User.CanAccessUser(id))
            return Forbid();

        var result = await sender.Send(new GetUserByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateUserCommand(id, request.FullName, request.IsActive, request.PromoteToAdmin), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteUserCommand(id), ct);
        return NoContent();
    }
}

public record UpdateUserRequest(
    string FullName,
    bool IsActive,
    bool PromoteToAdmin = false);
