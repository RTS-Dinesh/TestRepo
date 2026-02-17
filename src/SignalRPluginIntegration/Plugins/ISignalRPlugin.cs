using Microsoft.AspNetCore.SignalR;

namespace SignalRPluginIntegration.Plugins;

public interface ISignalRPlugin
{
    Task OnConnectedAsync(HubLifetimeContext context, CancellationToken cancellationToken);

    Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        CancellationToken cancellationToken);

    Task OnBeforeHubMethodInvocationAsync(
        HubInvocationContext context,
        CancellationToken cancellationToken);

    Task OnAfterHubMethodInvocationAsync(
        HubInvocationContext context,
        object? result,
        CancellationToken cancellationToken);
}
