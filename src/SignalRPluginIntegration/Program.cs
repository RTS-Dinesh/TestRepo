using SignalRPluginIntegration.Extensions;
using SignalRPluginIntegration.Hubs;
using SignalRPluginIntegration.Plugins;
using SignalRPluginIntegration.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSignalRWithPluginPipeline()
    .AddSignalRPlugin<ConnectionAuditPlugin>()
    .AddSignalRPlugin<ProfanityGuardPlugin>();

builder.Services.AddSingleton<INotificationBroadcaster, NotificationBroadcaster>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    service = "SignalR Plugin Integration",
    status = "running"
}));

app.MapPost(
    "/notifications/broadcast",
    async (BroadcastRequest request, INotificationBroadcaster broadcaster, CancellationToken cancellationToken) =>
    {
        await broadcaster.BroadcastAsync(request.User, request.Message, cancellationToken);
        return Results.Accepted();
    });

app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();

public sealed record BroadcastRequest(string User, string Message);
