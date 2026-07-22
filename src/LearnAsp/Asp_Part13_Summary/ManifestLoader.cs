using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Part13_Summary;

public sealed class ManifestLoader
{
    private readonly CapabilitiesFile? _capabilities;
    private readonly CapstonesFile? _capstones;
    private readonly InfrastructureFile? _infrastructure;
    private readonly EvidenceFile? _evidence;

    public ManifestLoader(IWebHostEnvironment environment)
    {
        string[] candidates = new[]
        {
            Path.Join(environment.ContentRootPath, "manifests"),
            Path.Join(AppContext.BaseDirectory, "manifests"),
            Path.Join(Path.Join(environment.ContentRootPath, "..", "..", ".."), "..", "..", "docs", "summary"),
        };
        string manifestsDir = candidates.FirstOrDefault(Directory.Exists)
            ?? Path.Join(environment.ContentRootPath, "manifests");
        _capabilities = Load<CapabilitiesFile>(Path.Join(manifestsDir, "capabilities.json"));
        _capstones = Load<CapstonesFile>(Path.Join(manifestsDir, "capstones.json"));
        _infrastructure = Load<InfrastructureFile>(Path.Join(manifestsDir, "infrastructure.json"));
        _evidence = Load<EvidenceFile>(Path.Join(manifestsDir, "evidence.json"));
    }

    public CapabilitiesFile? Capabilities => _capabilities;
    public CapstonesFile? Capstones => _capstones;
    public InfrastructureFile? Infrastructure => _infrastructure;
    public EvidenceFile? Evidence => _evidence;

    private static T? Load<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            return null;
        }
        string json = File.ReadAllText(path);
        JsonTypeInfo? typeInfo = SummaryJsonContext.Default.GetTypeInfo(typeof(T));
        if (typeInfo is null)
        {
            return null;
        }
        return JsonSerializer.Deserialize(json, typeInfo) as T;
    }
}
