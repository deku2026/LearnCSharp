// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : ConfigurationAndOptions
// Topic id : stage09/section06/configuration_and_options
//
// 步骤 3：分层配置 + Options 模式（BCL 教育版，无 Microsoft.Extensions.Configuration 包）

using System.Diagnostics;
using System.Text.Json;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section06;

internal static class ConfigurationAndOptions
{
    private sealed class AppOptions
    {
        public string AppName { get; set; } = "";
        public int MaxItems { get; set; }
        public bool FeatureX { get; set; }
    }

    /// <summary>Layered key-value config: later sources override earlier ones.</summary>
    private sealed class MiniConfiguration
    {
        private readonly Dictionary<string, string> _data = new(StringComparer.OrdinalIgnoreCase);

        public void AddInMemory(IEnumerable<KeyValuePair<string, string>> pairs)
        {
            foreach (KeyValuePair<string, string> kv in pairs)
                _data[kv.Key] = kv.Value;
        }

        public void AddJsonFile(string path)
        {
            if (!File.Exists(path)) return;
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path));
            Flatten(doc.RootElement, prefix: null);
        }

        public void AddEnvironmentVariables(string prefix)
        {
            foreach (System.Collections.DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                string key = (string)entry.Key;
                if (!key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;
                string configKey = key[prefix.Length..].Replace("__", ":", StringComparison.Ordinal);
                _data[configKey] = entry.Value?.ToString() ?? "";
            }
        }

        public string? this[string key] => _data.TryGetValue(key, out string? v) ? v : null;

        public T GetOptions<T>() where T : new()
        {
            // bind AppName / MaxItems style flat keys under section "App"
            var opts = new T();
            if (opts is AppOptions app)
            {
                if (this["App:AppName"] is string name) app.AppName = name;
                if (this["App:MaxItems"] is string max && int.TryParse(max, out int n)) app.MaxItems = n;
                if (this["App:FeatureX"] is string fx && bool.TryParse(fx, out bool b)) app.FeatureX = b;
            }
            return opts;
        }

        private void Flatten(JsonElement el, string? prefix)
        {
            if (el.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty p in el.EnumerateObject())
                {
                    string key = prefix is null ? p.Name : $"{prefix}:{p.Name}";
                    Flatten(p.Value, key);
                }
            }
            else if (el.ValueKind is JsonValueKind.String or JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False)
            {
                if (prefix is not null)
                    _data[prefix] = el.ToString();
            }
        }
    }

    [LearnTopic("stage09/section06/configuration_and_options")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ConfigurationAndOptions ===");
        DemoLayeredConfig();
        DemoOptionsPattern();
        return 0;
    }

    private static void DemoLayeredConfig()
    {
        Console.WriteLine("-- layered: defaults < json < env --");
        string path = Path.Combine(Path.GetTempPath(), $"learn-cfg-{Guid.NewGuid():N}.json");
        try
        {
            File.WriteAllText(path, """
                {
                  "App": {
                    "AppName": "FromJson",
                    "MaxItems": 10,
                    "FeatureX": false
                  }
                }
                """);

            var config = new MiniConfiguration();
            config.AddInMemory(new Dictionary<string, string>
            {
                ["App:AppName"] = "Defaults",
                ["App:MaxItems"] = "5"
            });
            config.AddJsonFile(path);
            // env would win if set: LearnCSharp_App__AppName etc.
            config.AddEnvironmentVariables("LearnCSharp_");

            Debug.Assert(config["App:AppName"] == "FromJson");
            Debug.Assert(config["App:MaxItems"] == "10");
            Console.WriteLine($"  AppName={config["App:AppName"]}; MaxItems={config["App:MaxItems"]}");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    private static void DemoOptionsPattern()
    {
        Console.WriteLine("-- Options: bind to strongly-typed POCO --");
        var config = new MiniConfiguration();
        config.AddInMemory(new Dictionary<string, string>
        {
            ["App:AppName"] = "LearnCSharp",
            ["App:MaxItems"] = "42",
            ["App:FeatureX"] = "true"
        });
        AppOptions opts = config.GetOptions<AppOptions>();
        Debug.Assert(opts is { AppName: "LearnCSharp", MaxItems: 42, FeatureX: true });
        Console.WriteLine($"  IOptions-like: {opts.AppName}, MaxItems={opts.MaxItems}, FeatureX={opts.FeatureX}");
        Console.WriteLine("  real stack: IConfiguration + IOptions<T> / IOptionsMonitor<T>");
    }
}
