using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace LearnMaui.Maui_Stage19_TrimmingAOT;

public sealed record ReleaseArticle(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("title")] string Title);

[JsonSerializable(typeof(ReleaseArticle))]
[JsonSerializable(typeof(IReadOnlyList<ReleaseArticle>))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class ReleaseJsonContext : JsonSerializerContext;

public static partial class InputPatterns
{
    [GeneratedRegex(
        "^[A-Za-z0-9_-]{1,64}$",
        RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 100)]
    public static partial Regex SafeIdentifier();
}

public sealed class AotFriendlyViewFactory
{
    private readonly IReadOnlyDictionary<string, Func<Page>> _factories =
        new Dictionary<string, Func<Page>>(StringComparer.OrdinalIgnoreCase)
        {
            ["home"] = static () => new ContentPage { Title = "Home" },
            ["settings"] = static () => new ContentPage { Title = "Settings" },
            ["about"] = static () => new ContentPage { Title = "About" },
        };

    public IReadOnlyCollection<string> Keys => [.. _factories.Keys];

    public Page Create(string key)
    {
        return _factories.TryGetValue(key, out Func<Page>? factory)
            ? factory()
            : throw new KeyNotFoundException($"Unknown view key: {key}");
    }
}

public static class TrimSafeActivator
{
    public static T Create<
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        T>()
        where T : class
    {
        return Activator.CreateInstance<T>();
    }

    [RequiresUnreferencedCode(
        "Convention-based discovery needs an explicit preservation contract.")]
    [RequiresDynamicCode(
        "Some dynamically discovered types may require runtime code generation.")]
    public static IReadOnlyList<Type> DiscoverByConvention(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(type => type.Name.EndsWith("Plugin", StringComparison.Ordinal))
            .ToArray();
    }

    [DynamicDependency(
        DynamicallyAccessedMemberTypes.PublicProperties,
        typeof(LegacyPayload))]
    public static IReadOnlyList<string> GetPreservedPropertyNames()
    {
        return typeof(LegacyPayload)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(property => property.Name)
            .ToArray();
    }
}

public sealed class LegacyPayload
{
    public string Name { get; set; } = string.Empty;

    public int Version { get; set; }
}

public sealed record RuntimeCapabilities(
    bool DynamicCodeSupported,
    bool DynamicCodeCompiled,
    string Framework,
    string Architecture)
{
    public static RuntimeCapabilities Capture()
    {
        return new RuntimeCapabilities(
            RuntimeFeature.IsDynamicCodeSupported,
            RuntimeFeature.IsDynamicCodeCompiled,
            System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString());
    }
}

public static class TrimSafeJson
{
    public static string Serialize(ReleaseArticle article)
    {
        return JsonSerializer.Serialize(article, ReleaseJsonContext.Default.ReleaseArticle);
    }

    public static ReleaseArticle? Deserialize(string json)
    {
        return JsonSerializer.Deserialize(
            json,
            ReleaseJsonContext.Default.ReleaseArticle);
    }
}
