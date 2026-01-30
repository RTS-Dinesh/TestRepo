using Confluent.Kafka;

const string usage = """
Kafka Consumer (System B)

Usage:
  dotnet run --project src/Consumer -- --bootstrap-servers localhost:9092 --topic demo-topic [--group demo-group] [--output-file messages.log]

Environment:
  KAFKA_BOOTSTRAP_SERVERS, KAFKA_TOPIC, KAFKA_GROUP_ID
""";

var argsMap = ParseArgs(args);
if (argsMap.ContainsKey("help"))
{
    Console.WriteLine(usage);
    return;
}

var bootstrapServers = GetRequired(argsMap, "bootstrap-servers") ?? Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
var topic = GetRequired(argsMap, "topic") ?? Environment.GetEnvironmentVariable("KAFKA_TOPIC");
var groupId = GetOptional(argsMap, "group") ?? Environment.GetEnvironmentVariable("KAFKA_GROUP_ID") ?? "message-transfer-consumer";
var outputFile = GetOptional(argsMap, "output-file");

if (string.IsNullOrWhiteSpace(bootstrapServers) || string.IsNullOrWhiteSpace(topic))
{
    Console.WriteLine(usage);
    return;
}

var config = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false,
};

using var consumer = new ConsumerBuilder<string, string>(config)
    .SetKeyDeserializer(Deserializers.Utf8)
    .SetValueDeserializer(Deserializers.Utf8)
    .Build();

using var outputWriter = string.IsNullOrWhiteSpace(outputFile)
    ? null
    : new StreamWriter(outputFile, append: true);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    Console.WriteLine("Stopping consumer...");
    cts.Cancel();
};

consumer.Subscribe(topic);
Console.WriteLine($"Consuming from {topic} as {groupId}...");

while (!cts.IsCancellationRequested)
{
    try
    {
        var result = consumer.Consume(cts.Token);
        var payload = $"[{result.TopicPartitionOffset}] key={result.Message.Key ?? "<null>"} value={result.Message.Value}";
        Console.WriteLine(payload);

        if (outputWriter != null)
        {
            outputWriter.WriteLine(payload);
            outputWriter.Flush();
        }

        consumer.Commit(result);
    }
    catch (ConsumeException ex)
    {
        Console.Error.WriteLine($"Consume error: {ex.Error.Reason}");
    }
    catch (OperationCanceledException)
    {
        break;
    }
}

consumer.Close();

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
