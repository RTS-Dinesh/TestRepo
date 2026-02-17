# SignalR plugin integration in .NET

This repository contains a ready-to-use ASP.NET Core SignalR setup with a plugin pipeline.

## What is included

- SignalR hub (`NotificationsHub`)
- Plugin contract (`ISignalRPlugin`)
- Hub filter (`SignalRPluginHubFilter`) that executes all plugins
- Registration extensions for clean integration in `Program.cs`
- Example plugins:
  - `ConnectionAuditPlugin` for logging connection and invocation activity
  - `ProfanityGuardPlugin` for simple message validation
- A REST endpoint (`POST /notifications/broadcast`) that pushes messages to SignalR clients

## Project structure

```text
src/SignalRPluginIntegration/
  Extensions/SignalRPluginExtensions.cs
  Hubs/NotificationsHub.cs
  Plugins/
    ISignalRPlugin.cs
    SignalRPluginHubFilter.cs
    ConnectionAuditPlugin.cs
    ProfanityGuardPlugin.cs
  Services/NotificationBroadcaster.cs
  Program.cs
  SignalRPluginIntegration.csproj
```

## Integration in .NET (`Program.cs`)

```csharp
builder.Services
    .AddSignalRWithPluginPipeline()
    .AddSignalRPlugin<ConnectionAuditPlugin>()
    .AddSignalRPlugin<ProfanityGuardPlugin>();

app.MapHub<NotificationsHub>("/hubs/notifications");
```

## Add your own plugin

Create a class that implements `ISignalRPlugin`, then register it:

```csharp
builder.Services
    .AddSignalRWithPluginPipeline()
    .AddSignalRPlugin<MyCustomSignalRPlugin>();
```

## Run

```bash
dotnet restore src/SignalRPluginIntegration/SignalRPluginIntegration.csproj
dotnet run --project src/SignalRPluginIntegration/SignalRPluginIntegration.csproj
```

## Quick test

Broadcast from REST:

```bash
curl -X POST http://localhost:5000/notifications/broadcast \
  -H "Content-Type: application/json" \
  -d '{"user":"system","message":"hello from api"}'
```

Connect a JavaScript client:

```javascript
const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/hubs/notifications")
  .build();

connection.on("ReceiveMessage", (user, message) => {
  console.log(`${user}: ${message}`);
});

await connection.start();
await connection.invoke("SendToEveryone", "browser", "hello from hub");
```
