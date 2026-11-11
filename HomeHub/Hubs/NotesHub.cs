using HomeHub.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomeHub.Hubs;

[Authorize]
public class NotesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{Context.User.GetUserId()}");
        }

        await base.OnConnectedAsync();
    }

    public static string GroupName(int userId)
    {
        return $"user-{userId}";
    }
}
