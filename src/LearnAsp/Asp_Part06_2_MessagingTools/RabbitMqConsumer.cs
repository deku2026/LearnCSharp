using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Part06_2_MessagingTools;

public sealed class RabbitMqConsumer(
    RabbitMqConnection connection,
    RabbitInboxStore inbox,
    IConfiguration configuration,
    ILogger<RabbitMqConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!configuration.GetValue("RabbitMQ:RunConsumer", true))
        {
            return;
        }

        await inbox.InitializeAsync(stoppingToken);
        await using IChannel channel =
            await connection.CreateConsumerChannelAsync(stoppingToken);
        ushort prefetch = (ushort)Math.Clamp(
            configuration.GetValue("RabbitMQ:Prefetch", 8),
            1,
            ushort.MaxValue);
        int maxAttempts = Math.Max(
            1,
            configuration.GetValue("RabbitMQ:MaxDeliveryAttempts", 3));
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: prefetch,
            global: false,
            cancellationToken: stoppingToken);

        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, delivery) =>
        {
            // RabbitMQ.Client 7 owns the ReadOnlyMemory only for this callback.
            byte[] body = delivery.Body.ToArray();
            try
            {
                RabbitLabMessage message = JsonSerializer.Deserialize<RabbitLabMessage>(body)
                    ?? throw new JsonException("RabbitMQ message body was empty.");
                int attempt = ReadAttempt(delivery.BasicProperties.Headers);
                if (string.Equals(
                        message.FailureMode,
                        "poison",
                        StringComparison.OrdinalIgnoreCase))
                {
                    await channel.BasicRejectAsync(
                        delivery.DeliveryTag,
                        requeue: false,
                        stoppingToken);
                    return;
                }

                if (string.Equals(
                        message.FailureMode,
                        "transient",
                        StringComparison.OrdinalIgnoreCase) &&
                    attempt < message.FailuresBeforeSuccess)
                {
                    if (attempt + 1 >= maxAttempts)
                    {
                        await channel.BasicRejectAsync(
                            delivery.DeliveryTag,
                            requeue: false,
                            stoppingToken);
                        return;
                    }

                    await connection.PublishRetryAsync(
                        message,
                        attempt + 1,
                        stoppingToken);
                    await channel.BasicAckAsync(
                        delivery.DeliveryTag,
                        multiple: false,
                        stoppingToken);
                    return;
                }

                bool accepted = await inbox.TryHandleAsync(
                    message,
                    attempt,
                    stoppingToken);
                logger.LogInformation(
                    accepted
                        ? "Handled RabbitMQ message {MessageId} at attempt {Attempt}."
                        : "Ignored duplicate RabbitMQ message {MessageId}.",
                    message.MessageId,
                    attempt);
                await channel.BasicAckAsync(
                    delivery.DeliveryTag,
                    multiple: false,
                    stoppingToken);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Malformed RabbitMQ message moved to DLQ.");
                await channel.BasicRejectAsync(
                    delivery.DeliveryTag,
                    requeue: false,
                    stoppingToken);
            }
            catch (Npgsql.NpgsqlException ex)
            {
                logger.LogWarning(
                    ex,
                    "PostgreSQL unavailable; RabbitMQ message will be redelivered.");
                await channel.BasicNackAsync(
                    delivery.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    stoppingToken);
            }
        };

        string consumerTag = await channel.BasicConsumeAsync(
            RabbitMqTopology.NotificationsQueue,
            autoAck: false,
            consumer,
            stoppingToken);
        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            await channel.BasicCancelAsync(
                consumerTag,
                noWait: false,
                CancellationToken.None);
        }
    }

    private static int ReadAttempt(IDictionary<string, object?>? headers)
    {
        if (headers is null || !headers.TryGetValue("x-attempt", out object? raw))
        {
            return 0;
        }

        return raw switch
        {
            byte value => value,
            short value => value,
            int value => value,
            long value => checked((int)value),
            _ => 0,
        };
    }
}
