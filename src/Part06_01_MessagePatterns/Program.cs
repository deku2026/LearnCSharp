using System.Collections.Concurrent;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<InMemoryBus>();
builder.Services.AddSingleton<Outbox>();
builder.Services.AddHostedService<OutboxDispatcher>();
var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { lab="Part06_01 Message patterns: command/event/outbox" }));
app.MapPost("/commands/place-order", (PlaceOrder cmd, Outbox outbox) => {
  // Command handled, event written to outbox atomically (in-memory demo)
  var evt = new OrderPlaced(Guid.NewGuid(), cmd.StudentNumber, cmd.Sku);
  outbox.Add(evt);
  return Results.Accepted($"/orders/{evt.OrderId}", evt);
});
app.MapGet("/events", (InMemoryBus bus) => bus.All());
app.MapGet("/health", () => Results.Ok(new { status="Healthy" }));
app.Run();
public partial class Program;
public record PlaceOrder(string StudentNumber, string Sku);
public record OrderPlaced(Guid OrderId, string StudentNumber, string Sku);
public sealed class Outbox { private readonly ConcurrentQueue<object> _q=new(); public void Add(object e)=>_q.Enqueue(e); public bool TryDequeue(out object? e)=>_q.TryDequeue(out e); }
public sealed class InMemoryBus { private readonly ConcurrentBag<object> _bag=new(); public void Publish(object e)=>_bag.Add(e); public object All()=>_bag.ToArray(); }
public sealed class OutboxDispatcher(Outbox outbox, InMemoryBus bus, ILogger<OutboxDispatcher> log): BackgroundService {
  protected override async Task ExecuteAsync(CancellationToken ct) {
    while(!ct.IsCancellationRequested) {
      if(outbox.TryDequeue(out var e) && e is not null) { bus.Publish(e); log.LogInformation("Dispatched {Type}", e.GetType().Name); }
      await Task.Delay(50, ct);
    }
  }
}
