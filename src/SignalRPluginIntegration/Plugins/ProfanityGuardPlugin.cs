using Microsoft.AspNetCore.SignalR;

namespace SignalRPluginIntegration.Plugins;

public sealed class ProfanityGuardPlugin : ISignalRPlugin
{
    private static readonly string[] BlockedTerms = ["badword", "forbidden"];

    public Task OnConnectedAsync(HubLifetimeContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task OnBeforeHubMethodInvocationAsync(
        HubInvocationContext context,
        CancellationToken cancellationToken)
    {
        if (!context.HubMethodName.Equals("SendToEveryone", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        var message = context.HubMethodArguments.OfType<string>().LastOrDefault();

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new HubException("Message cannot be empty.");
        }

        if (BlockedTerms.Any(term => message.Contains(term, StringComparison.OrdinalIgnoreCase)))
        {
            throw new HubException("Message contains disallowed terms.");
        }

        return Task.CompletedTask;
    }

    public Task OnAfterHubMethodInvocationAsync(
        HubInvocationContext context,
        object? result,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
