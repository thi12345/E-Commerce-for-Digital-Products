using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using ShopApp.API.Auth;
using ShopApp.API.Hubs;
using ShopApp.Application.Auth.Commands.ChangePassword;
using ShopApp.Application.Auth.Commands.Login;
using ShopApp.Application.Auth.Commands.Register;
using ShopApp.Application.Users.Queries.GetUserById;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(ISender sender, IHubContext<AuthSessionHub> authSessionHub) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(Me), null, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var user = await sender.Send(new GetUserByIdQuery(userId.Value), ct);
        return user is null ? Unauthorized() : Ok(user);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        await sender.Send(new ChangePasswordCommand(userId.Value, request.CurrentPassword, request.NewPassword), ct);
        await authSessionHub.Clients
            .Group(AuthSessionHub.UserGroup(userId.Value))
            .SendAsync(AuthSessionHub.SessionRevokedEvent, new SessionRevokedMessage("PasswordChanged"), ct);

        return NoContent();
    }
}

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);

public record SessionRevokedMessage(string Reason);
