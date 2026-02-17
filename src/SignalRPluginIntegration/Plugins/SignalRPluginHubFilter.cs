using Microsoft.AspNetCore.SignalR;

namespace SignalRPluginIntegration.Plugins;

public sealed class SignalRPluginHubFilter(IEnumerable<ISignalRPlugin> plugins) : IHubFilter
{
    private readonly IReadOnlyList<ISignalRPlugin> _plugins = plugins.ToList();

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var cancellationToken = invocationContext.Context.ConnectionAborted;

        foreach (var plugin in _plugins)
        {
            await plugin.OnBeforeHubMethodInvocationAsync(invocationContext, cancellationToken);
        }

        var result = await next(invocationContext);

        foreach (var plugin in _plugins)
        {
            await plugin.OnAfterHubMethodInvocationAsync(invocationContext, result, cancellationToken);
        }

        return result;
    }

    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        var cancellationToken = context.Context.ConnectionAborted;

        foreach (var plugin in _plugins)
        {
            await plugin.OnConnectedAsync(context, cancellationToken);
        }

        await next(context);
    }

    public async Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        var cancellationToken = context.Context.ConnectionAborted;

        foreach (var plugin in _plugins)
        {
            await plugin.OnDisconnectedAsync(context, exception, cancellationToken);
        }

        await next(context, exception);
    }
}
