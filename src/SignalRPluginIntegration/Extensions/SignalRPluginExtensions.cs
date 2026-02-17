using Microsoft.AspNetCore.SignalR;
using SignalRPluginIntegration.Plugins;

namespace SignalRPluginIntegration.Extensions;

public static class SignalRPluginExtensions
{
    public static ISignalRServerBuilder AddSignalRWithPluginPipeline(this IServiceCollection services)
    {
        services.AddSingleton<SignalRPluginHubFilter>();

        return services.AddSignalR(options =>
        {
            options.AddFilter<SignalRPluginHubFilter>();
        });
    }

    public static ISignalRServerBuilder AddSignalRPlugin<TPlugin>(this ISignalRServerBuilder builder)
        where TPlugin : class, ISignalRPlugin
    {
        builder.Services.AddSingleton<ISignalRPlugin, TPlugin>();
        return builder;
    }
}
