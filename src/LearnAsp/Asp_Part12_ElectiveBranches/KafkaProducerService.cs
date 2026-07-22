using System.Text.Json;
using Confluent.Kafka;

namespace Part12_ElectiveBranches;

public sealed class KafkaProducerService : IAsyncDisposable
{
    private readonly ProducerConfig _config;
    private readonly string _topic;
    private readonly ILogger<KafkaProducerService> _logger;
    private IProducer<string, string>? _producer;
    private int _disposeState;

    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        string bootstrap = configuration.GetConnectionString("Kafka") ?? "localhost:9094";
        _topic = KafkaTopics.EnrollmentActivity;
        _logger = logger;
        _config = new ProducerConfig
        {
            BootstrapServers = bootstrap,
            Acks = Acks.All,
            EnableIdempotence = true,
            ClientId = "campus-w9-producer",
        };
    }

    private IProducer<string, string> GetProducer() =>
        _producer ??= new ProducerBuilder<string, string>(_config).Build();

    public async Task<KafkaProduceResult> PublishAsync(
        EnrollmentActivityEvent evt,
        CancellationToken cancellationToken)
    {
        string key = evt.EnrollmentId.ToString();
        string value = JsonSerializer.Serialize(evt);
        DeliveryResult<string, string> result = await GetProducer().ProduceAsync(_topic, new Message<string, string>
        {
            Key = key,
            Value = value,
        }, cancellationToken);
        _logger.LogInformation(
            "Published enrollment activity {EnrollmentId} to {Topic} partition {Partition} offset {Offset}",
            evt.EnrollmentId, _topic, result.Partition, result.Offset);
        return new KafkaProduceResult(evt.EnrollmentId, _topic, result.Partition, result.Offset);
    }

    public async Task PublishToDlqAsync(
        EnrollmentActivityEvent evt,
        string reason,
        CancellationToken cancellationToken)
    {
        string value = JsonSerializer.Serialize(new { evt, reason });
        await GetProducer().ProduceAsync(KafkaTopics.EnrollmentActivityDlq, new Message<string, string>
        {
            Key = evt.EnrollmentId.ToString(),
            Value = value,
        }, cancellationToken);
        _logger.LogWarning(
            "Enrollment activity {EnrollmentId} sent to DLQ: {Reason}",
            evt.EnrollmentId, reason);
    }

    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposeState, 1) != 0)
        {
            return ValueTask.CompletedTask;
        }
        _producer?.Dispose();
        return ValueTask.CompletedTask;
    }
}
