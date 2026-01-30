# Kafka C# Message Transfer

This repo shows a minimal Kafka producer (System A) and consumer (System B)
implemented in C# to transfer messages between systems.

## Prerequisites

- .NET 8 SDK
- A running Kafka broker (Bootstrap Servers address)

## Quick start

Set the bootstrap servers and topic (or pass them as args):

```bash
export KAFKA_BOOTSTRAP_SERVERS=localhost:9092
export KAFKA_TOPIC=demo-topic
```

Start the consumer (System B) in one terminal:

```bash
dotnet run --project src/Consumer -- --group demo-group --output-file messages.log
```

Start the producer (System A) in another terminal:

```bash
dotnet run --project src/Producer -- --message "hello from system A"
```

For interactive producer input, omit `--message` and type lines:

```bash
dotnet run --project src/Producer --
```

## CLI options

Producer:
- `--bootstrap-servers` (required unless env var set)
- `--topic` (required unless env var set)
- `--message` (single message; omit to read from stdin)
- `--key` (optional message key)

Consumer:
- `--bootstrap-servers` (required unless env var set)
- `--topic` (required unless env var set)
- `--group` (consumer group id)
- `--output-file` (append received messages to a file)
