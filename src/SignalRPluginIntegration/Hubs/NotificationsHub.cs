using Microsoft.AspNetCore.SignalR;

namespace SignalRPluginIntegration.Hubs;

public sealed class NotificationsHub : Hub
{
    public async Task SendToEveryone(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendToCaller(string message)
    {
        var user = Context.UserIdentifier ?? Context.ConnectionId;
        await Clients.Caller.SendAsync("ReceiveMessage", user, message);
    }
}
