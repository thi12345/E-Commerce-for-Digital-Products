using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShopApp.API.Auth;

namespace ShopApp.API.Hubs;

[Authorize]
public sealed class AuthSessionHub : Hub
{
    public const string Route = "/hubs/auth-session";
    public const string SessionRevokedEvent = "SessionRevoked";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.GetUserId();
        if (userId is null)
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId.Value));
        await base.OnConnectedAsync();
    }

    public static string UserGroup(Guid userId) => $"user:{userId:N}";
}
