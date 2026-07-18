namespace Step02_DiConfigOptions.Services;

public interface IOperation
{
    Guid OperationId { get; }
}

public interface IOperationTransient : IOperation;
public interface IOperationScoped : IOperation;
public interface IOperationSingleton : IOperation;

public sealed class Operation : IOperationTransient, IOperationScoped, IOperationSingleton
{
    public Guid OperationId { get; } = Guid.NewGuid();
}

public interface IGuidWriter
{
    string Name { get; }
    Guid Write();
}

public sealed class ConsoleGuidWriter : IGuidWriter
{
    public string Name => "console";
    public Guid Write() => Guid.NewGuid();
}

public sealed class FileGuidWriter : IGuidWriter
{
    public string Name => "file";
    public Guid Write() => Guid.NewGuid();
}

// Keyed services demo: multiple payment gateways
public interface IPaymentGateway
{
    string Charge(decimal amount);
}

public sealed class AlipayGateway : IPaymentGateway
{
    public string Charge(decimal amount) => $"alipay:{amount:F2}";
}

public sealed class WeChatPayGateway : IPaymentGateway
{
    public string Charge(decimal amount) => $"wechat:{amount:F2}";
}

// Decorator demo (Scrutor): log around IStudentDirectory
public interface IStudentDirectory
{
    IReadOnlyList<string> ListNames();
}

public sealed class InMemoryStudentDirectory : IStudentDirectory
{
    public IReadOnlyList<string> ListNames() => ["张三", "李四", "王五"];
}

public sealed class LoggingStudentDirectory(IStudentDirectory inner, ILogger<LoggingStudentDirectory> logger)
    : IStudentDirectory
{
    public IReadOnlyList<string> ListNames()
    {
        logger.LogInformation("Listing students via decorator");
        return inner.ListNames();
    }
}

/// <summary>Scoped "DbContext stand-in" — must not be captured by singletons.</summary>
public sealed class FakeDbContext
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

/// <summary>Background worker correctly uses IServiceScopeFactory for scoped FakeDbContext.</summary>
public sealed class ScopedSafeWorker(IServiceScopeFactory scopeFactory, ILogger<ScopedSafeWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        try
        {
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<FakeDbContext>();
                logger.LogInformation("ScopedSafeWorker used FakeDbContext {Id}", db.InstanceId);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }
}
