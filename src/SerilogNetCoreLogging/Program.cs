using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    app.MapGet("/", (ILogger<Program> logger) =>
    {
        logger.LogInformation("Handling root request at {UtcNow}", DateTime.UtcNow);

        return Results.Ok(new
        {
            message = "Hello from Serilog",
            time = DateTime.UtcNow
        });
    });

    app.MapGet("/error", () =>
    {
        throw new InvalidOperationException("Sample exception for Serilog");
    });

    Log.Information("Starting Serilog sample application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
