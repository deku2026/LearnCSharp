namespace Step03_MiddlewarePipeline.Services;

public sealed class RequestIdFactory
{
    public string Create() => Guid.NewGuid().ToString("N")[..8];
}

public sealed class ScopedRequestCounter
{
    public int Value { get; private set; }
    public void Increment() => Value++;
}
