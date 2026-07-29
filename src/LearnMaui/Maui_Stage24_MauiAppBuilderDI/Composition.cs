namespace LearnMaui.Maui_Stage24_MauiAppBuilderDI;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        services.AddSingleton(new EndpointOptions(
            new Uri("https://api.example.invalid"),
            TimeSpan.FromSeconds(15)));
        services.AddSingleton<AppSession>();
        services.AddScoped<PageSession>();
        services.AddTransient<OperationToken>();
        services.AddSingleton<LifetimeProbe>();
        services.AddSingleton<CompositionLedger>();
        return services;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddTransient<MainPage>();
        return services;
    }
}

public sealed record EndpointOptions(Uri BaseAddress, TimeSpan Timeout);

public sealed class AppSession
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class PageSession : IDisposable
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool Disposed { get; private set; }

    public void Dispose()
    {
        Disposed = true;
    }
}

public sealed class OperationToken
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed record LifetimeEvidence(
    bool SingletonSameAcrossScopes,
    bool ScopedSameWithinScope,
    bool ScopedDifferentAcrossScopes,
    bool TransientDifferent,
    Guid SingletonId,
    Guid FirstScopeId,
    Guid SecondScopeId);

public sealed class LifetimeProbe(IServiceScopeFactory scopeFactory)
{
    public LifetimeEvidence Inspect()
    {
        using IServiceScope first = scopeFactory.CreateScope();
        using IServiceScope second = scopeFactory.CreateScope();

        AppSession singletonA = first.ServiceProvider.GetRequiredService<AppSession>();
        AppSession singletonB = second.ServiceProvider.GetRequiredService<AppSession>();
        PageSession scopedA1 = first.ServiceProvider.GetRequiredService<PageSession>();
        PageSession scopedA2 = first.ServiceProvider.GetRequiredService<PageSession>();
        PageSession scopedB = second.ServiceProvider.GetRequiredService<PageSession>();
        OperationToken transientA = first.ServiceProvider.GetRequiredService<OperationToken>();
        OperationToken transientB = first.ServiceProvider.GetRequiredService<OperationToken>();

        return new LifetimeEvidence(
            ReferenceEquals(singletonA, singletonB),
            ReferenceEquals(scopedA1, scopedA2),
            !ReferenceEquals(scopedA1, scopedB),
            !ReferenceEquals(transientA, transientB),
            singletonA.Id,
            scopedA1.Id,
            scopedB.Id);
    }
}

public enum CompositionPhase
{
    Created,
    FrameworkConfigured,
    ServicesRegistered,
    Built,
}

public sealed class CompositionLedger
{
    private readonly List<string> _registrations = [];

    public CompositionPhase Phase { get; private set; } = CompositionPhase.Created;

    public IReadOnlyList<string> Registrations => _registrations;

    public void Advance(CompositionPhase next)
    {
        if (next <= Phase)
        {
            throw new InvalidOperationException(
                $"Composition can only move forward: {Phase} -> {next}.");
        }

        Phase = next;
    }

    public void Register(string descriptor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(descriptor);
        if (Phase == CompositionPhase.Built)
        {
            throw new InvalidOperationException(
                "The service graph is immutable after Build.");
        }

        _registrations.Add(descriptor);
    }
}

public static class StartupCallChain
{
    public static string DescribeAndroid() =>
        "MainActivity/Android runtime -> MauiApplication.CreateMauiApp " +
        "-> MauiProgram.CreateMauiApp -> MauiAppBuilder.Build " +
        "-> root IServiceProvider -> App -> Window -> Element.Handler -> MauiContext.Services";
}
