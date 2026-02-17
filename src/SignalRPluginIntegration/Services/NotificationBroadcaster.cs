using Microsoft.AspNetCore.SignalR;
using SignalRPluginIntegration.Hubs;

namespace SignalRPluginIntegration.Services;

public interface INotificationBroadcaster
{
    Task BroadcastAsync(string user, string message, CancellationToken cancellationToken);
}

public sealed class NotificationBroadcaster(IHubContext<NotificationsHub> hubContext) : INotificationBroadcaster
{
    public Task BroadcastAsync(string user, string message, CancellationToken cancellationToken)
    {
        return hubContext.Clients.All.SendAsync("ReceiveMessage", user, message, cancellationToken);
    }
}
