using Confluent.Kafka;

static string GetEnv(string name, string defaultValue)
{
    var value = Environment.GetEnvironmentVariable(name);
    return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
}

static int GetEnvInt(string name, int defaultValue)
{
    var value = Environment.GetEnvironmentVariable(name);
    return int.TryParse(value, out var parsed) ? parsed : defaultValue;
}

var bootstrapServers = GetEnv("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092");
var topic = GetEnv("KAFKA_TOPIC", "demo-topic");
var groupId = GetEnv("KAFKA_GROUP_ID", "demo-consumer-group");
var pollMs = GetEnvInt("KAFKA_POLL_MS", 1000);

var config = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true,
};

using var consumer = new ConsumerBuilder<Ignore, string>(config)
    .SetErrorHandler((_, error) =>
    {
        if (error.IsError)
        {
            Console.Error.WriteLine($"Consumer error: {error.Reason}");
        }
    })
    .Build();

consumer.Subscribe(topic);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

Console.WriteLine("Kafka consumer starting.");
Console.WriteLine($"BootstrapServers: {bootstrapServers}");
Console.WriteLine($"Topic: {topic}");
Console.WriteLine($"GroupId: {groupId}");
Console.WriteLine("Press Ctrl+C to stop.");

try
{
    while (!cts.IsCancellationRequested)
    {
        try
        {
            var result = consumer.Consume(TimeSpan.FromMilliseconds(pollMs));
            if (result == null)
            {
                continue;
            }

            Console.WriteLine(
                $"Received {result.TopicPartitionOffset}: {result.Message.Value}");
        }
        catch (ConsumeException ex)
        {
            Console.Error.WriteLine($"Consume failed: {ex.Error.Reason}");
        }
    }
}
finally
{
    consumer.Close();
    Console.WriteLine("Kafka consumer stopped.");
}
