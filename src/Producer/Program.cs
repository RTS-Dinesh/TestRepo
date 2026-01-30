using Confluent.Kafka;

const string usage = """
Kafka Producer (System A)

Usage:
  dotnet run --project src/Producer -- --bootstrap-servers localhost:9092 --topic demo-topic [--message "hello"] [--key "k1"]

If --message is omitted, the producer reads lines from stdin.
Environment:
  KAFKA_BOOTSTRAP_SERVERS, KAFKA_TOPIC
""";

var argsMap = ParseArgs(args);
if (argsMap.ContainsKey("help"))
{
    Console.WriteLine(usage);
    return;
}

var bootstrapServers = GetRequired(argsMap, "bootstrap-servers") ?? Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
var topic = GetRequired(argsMap, "topic") ?? Environment.GetEnvironmentVariable("KAFKA_TOPIC");

if (string.IsNullOrWhiteSpace(bootstrapServers) || string.IsNullOrWhiteSpace(topic))
{
    Console.WriteLine(usage);
    return;
}

var message = GetOptional(argsMap, "message");
var key = GetOptional(argsMap, "key");

var config = new ProducerConfig
{
    BootstrapServers = bootstrapServers,
    Acks = Acks.All,
    EnableIdempotence = true,
    MessageTimeoutMs = 30000,
};

using var producer = new ProducerBuilder<string, string>(config)
    .SetKeySerializer(Serializers.Utf8)
    .SetValueSerializer(Serializers.Utf8)
    .Build();

async Task SendAsync(string value)
{
    try
    {
        var delivery = await producer.ProduceAsync(
            topic,
            new Message<string, string> { Key = key, Value = value });
        Console.WriteLine($"Delivered to {delivery.TopicPartitionOffset}");
    }
    catch (ProduceException<string, string> ex)
    {
        Console.Error.WriteLine($"Delivery failed: {ex.Error.Reason}");
    }
}

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    Console.WriteLine("Stopping producer...");
    producer.Flush(TimeSpan.FromSeconds(10));
    Environment.Exit(0);
};

if (!string.IsNullOrWhiteSpace(message))
{
    await SendAsync(message);
}
else
{
    Console.WriteLine("Type messages (Ctrl+C to exit):");
    string? line;
    while ((line = Console.ReadLine()) != null)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        await SendAsync(line);
    }
}

producer.Flush(TimeSpan.FromSeconds(10));

static Dictionary<string, string> ParseArgs(string[] args)
{
    var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    for (var i = 0; i < args.Length; i++)
    {
        var arg = args[i];
        if (!arg.StartsWith("--", StringComparison.Ordinal))
        {
            continue;
        }

        var key = arg[2..];
        var next = i + 1 < args.Length ? args[i + 1] : null;
        if (!string.IsNullOrEmpty(next) && !next.StartsWith("--", StringComparison.Ordinal))
        {
            map[key] = next;
            i++;
        }
        else
        {
            map[key] = "true";
        }
    }

    return map;
}

static string? GetRequired(IReadOnlyDictionary<string, string> argsMap, string key)
{
    return argsMap.TryGetValue(key, out var value) ? value : null;
}

static string? GetOptional(IReadOnlyDictionary<string, string> argsMap, string key)
{
    return argsMap.TryGetValue(key, out var value) ? value : null;
}
