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
var message = GetEnv("KAFKA_MESSAGE", "hello from kafka producer");
var count = GetEnvInt("KAFKA_COUNT", 10);
var delayMs = GetEnvInt("KAFKA_DELAY_MS", 500);

var config = new ProducerConfig
{
    BootstrapServers = bootstrapServers,
    Acks = Acks.All,
    EnableIdempotence = true,
};

using var producer = new ProducerBuilder<Null, string>(config).Build();

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

Console.WriteLine("Kafka producer starting.");
Console.WriteLine($"BootstrapServers: {bootstrapServers}");
Console.WriteLine($"Topic: {topic}");
Console.WriteLine($"Message: {message}");
Console.WriteLine($"Count: {count}, DelayMs: {delayMs}");

for (var i = 0; i < count && !cts.IsCancellationRequested; i++)
{
    var payload = $"{message} #{i + 1}";
    try
    {
        var result = await producer.ProduceAsync(
            topic,
            new Message<Null, string> { Value = payload },
            cts.Token);
        Console.WriteLine($"Delivered to {result.TopicPartitionOffset}");
    }
    catch (ProduceException<Null, string> ex)
    {
        Console.Error.WriteLine($"Delivery failed: {ex.Error.Reason}");
    }

    if (delayMs > 0)
    {
        try
        {
            await Task.Delay(delayMs, cts.Token);
        }
        catch (OperationCanceledException)
        {
            break;
        }
    }
}

producer.Flush(TimeSpan.FromSeconds(10));
Console.WriteLine("Kafka producer finished.");
