# Kafka C# Code Solution

This repository provides a minimal Kafka producer and consumer in C# using
Confluent.Kafka. It is intended as a concise starting point that you can
customize for your own workloads.

## Projects

- `src/KafkaProducer` - sends messages to a Kafka topic
- `src/KafkaConsumer` - reads messages from the same topic

## Requirements

- .NET 8 SDK
- Kafka broker reachable at `KAFKA_BOOTSTRAP_SERVERS`

## Quick start with Docker (optional)

If you want a local broker, you can use the included Docker Compose file:

```bash
docker compose up -d
```

## Run the consumer

```bash
dotnet run --project src/KafkaConsumer
```

## Run the producer

```bash
dotnet run --project src/KafkaProducer
```

## Configuration

All configuration is via environment variables. Defaults are shown below.

Producer:

- `KAFKA_BOOTSTRAP_SERVERS` (default: `localhost:9092`)
- `KAFKA_TOPIC` (default: `demo-topic`)
- `KAFKA_MESSAGE` (default: `hello from kafka producer`)
- `KAFKA_COUNT` (default: `10`)
- `KAFKA_DELAY_MS` (default: `500`)

Consumer:

- `KAFKA_BOOTSTRAP_SERVERS` (default: `localhost:9092`)
- `KAFKA_TOPIC` (default: `demo-topic`)
- `KAFKA_GROUP_ID` (default: `demo-consumer-group`)
- `KAFKA_POLL_MS` (default: `1000`)
