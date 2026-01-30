# Serilog .NET Core Logging Sample

This repository contains a minimal ASP.NET Core application that demonstrates
Serilog configuration and usage in .NET Core.

## Project Layout

- `src/SerilogNetCoreLogging`: Minimal Web API with Serilog configured via
  `appsettings.json`.

## What It Demonstrates

- Bootstrap logger for early startup messages
- Structured logging with `ILogger<T>` in endpoints
- Request logging middleware
- Console and rolling file sinks

## Run Locally

```bash
cd src/SerilogNetCoreLogging
dotnet restore
dotnet run
```

Endpoints:

- `GET /` returns a payload and logs a structured message
- `GET /error` throws an exception to show Serilog exception logging
