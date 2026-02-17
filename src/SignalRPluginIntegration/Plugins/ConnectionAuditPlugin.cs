using Microsoft.AspNetCore.SignalR;

namespace SignalRPluginIntegration.Plugins;

public sealed class ConnectionAuditPlugin(ILogger<ConnectionAuditPlugin> logger) : ISignalRPlugin
{
    public Task OnConnectedAsync(HubLifetimeContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "SignalR connection opened. ConnectionId: {ConnectionId}, User: {UserIdentifier}",
            context.Context.ConnectionId,
            context.Context.UserIdentifier ?? "anonymous");

        return Task.CompletedTask;
    }

    public Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            exception,
            "SignalR connection closed. ConnectionId: {ConnectionId}, User: {UserIdentifier}",
            context.Context.ConnectionId,
            context.Context.UserIdentifier ?? "anonymous");

        return Task.CompletedTask;
    }

    public Task OnBeforeHubMethodInvocationAsync(
        HubInvocationContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "SignalR hub method invoked. Method: {Method}, ConnectionId: {ConnectionId}",
            context.HubMethodName,
            context.Context.ConnectionId);

        return Task.CompletedTask;
    }

    public Task OnAfterHubMethodInvocationAsync(
        HubInvocationContext context,
        object? result,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "SignalR hub method completed. Method: {Method}, ConnectionId: {ConnectionId}",
            context.HubMethodName,
            context.Context.ConnectionId);

        return Task.CompletedTask;
    }
}
