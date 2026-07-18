using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);
var amqp = builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://dotnet:dotnet_dev@localhost:5672/";
builder.Services.AddSingleton<OrderPublisher>(_ => new OrderPublisher(amqp));
builder.Services.AddHostedService(sp => new OrderConsumerHostedService(amqp, sp.GetRequiredService<ILogger<OrderConsumerHostedService>>()));
builder.Services.AddSingleton<ConsumedStore>();

var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { lab="Part06_02 RabbitMQ", amqp }));
app.MapPost("/orders", async (PlaceOrderDto dto, OrderPublisher pub) =>
{
    var evt = new OrderPlaced(Guid.NewGuid(), dto.StudentNumber, dto.Sku, dto.Qty, DateTimeOffset.UtcNow);
    await pub.PublishAsync(evt);
    return Results.Accepted($"/orders/{evt.OrderId}", evt);
});
app.MapGet("/orders/consumed", (ConsumedStore store) => store.All());
app.Run();
public partial class Program;
public record PlaceOrderDto(string StudentNumber, string Sku, int Qty);
public record OrderPlaced(Guid OrderId, string StudentNumber, string Sku, int Qty, DateTimeOffset At);
public sealed class ConsumedStore { private readonly List<OrderPlaced> _items=new(); public void Add(OrderPlaced e){ lock(_items) _items.Add(e);} public object All(){ lock(_items) return _items.ToArray(); } }
public sealed class OrderPublisher(string amqp) {
  public async Task PublishAsync(OrderPlaced evt) {
    var factory = new ConnectionFactory { Uri = new Uri(amqp) };
    await using var conn = await factory.CreateConnectionAsync();
    await using var ch = await conn.CreateChannelAsync();
    await ch.ExchangeDeclareAsync("campusshop.events", ExchangeType.Topic, durable:true);
    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));
    await ch.BasicPublishAsync("campusshop.events", "order.placed", body);
  }
}
public sealed class OrderConsumerHostedService(string amqp, ILogger<OrderConsumerHostedService> log) : BackgroundService {
  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    var factory = new ConnectionFactory { Uri = new Uri(amqp) };
    await using var conn = await factory.CreateConnectionAsync(stoppingToken);
    await using var ch = await conn.CreateChannelAsync(cancellationToken: stoppingToken);
    await ch.ExchangeDeclareAsync("campusshop.events", ExchangeType.Topic, durable:true, cancellationToken: stoppingToken);
    var q = await ch.QueueDeclareAsync(queue: "campusshop.orders", durable:true, exclusive:false, autoDelete:false, cancellationToken: stoppingToken);
    await ch.QueueBindAsync(q.QueueName, "campusshop.events", "order.placed", cancellationToken: stoppingToken);
    var consumer = new AsyncEventingBasicConsumer(ch);
    consumer.ReceivedAsync += async (_, ea) => {
      var json = Encoding.UTF8.GetString(ea.Body.ToArray());
      log.LogInformation("Consumed order event: {Json}", json);
      await ch.BasicAckAsync(ea.DeliveryTag, false);
    };
    await ch.BasicConsumeAsync(q.QueueName, autoAck:false, consumer: consumer, cancellationToken: stoppingToken);
    try { await Task.Delay(Timeout.Infinite, stoppingToken); } catch (OperationCanceledException) {}
  }
}
