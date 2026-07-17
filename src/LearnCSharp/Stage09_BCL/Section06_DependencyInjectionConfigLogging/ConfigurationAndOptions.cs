// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : ConfigurationAndOptions
// Topic id : stage09/section06/configuration_and_options
//
// 步骤 3：IConfiguration + Options 模式（Microsoft.Extensions.Configuration / Options）

using System.Diagnostics;
using LearnCSharp.Topics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LearnCSharp.Stage09.Section06;

internal static class ConfigurationAndOptions
{
    private sealed class AppOptions
    {
        public const string SectionName = "App";
        public string AppName { get; set; } = "";
        public int MaxItems { get; set; }
        public bool FeatureX { get; set; }
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
        Console.WriteLine("-- layered: memory < json (later wins) --");
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

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["App:AppName"] = "Defaults",
                    ["App:MaxItems"] = "5"
                })
                .AddJsonFile(path, optional: false, reloadOnChange: false)
                .Build();

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
        Console.WriteLine("-- IOptions<T> bind from section --");
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["App:AppName"] = "LearnCSharp",
                ["App:MaxItems"] = "42",
                ["App:FeatureX"] = "true"
            })
            .Build();

        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddOptions<AppOptions>()
            .Bind(config.GetSection(AppOptions.SectionName));
        using ServiceProvider provider = services.BuildServiceProvider();

        AppOptions opts = provider.GetRequiredService<IOptions<AppOptions>>().Value;
        Debug.Assert(opts is { AppName: "LearnCSharp", MaxItems: 42, FeatureX: true });
        Console.WriteLine($"  IOptions: {opts.AppName}, MaxItems={opts.MaxItems}, FeatureX={opts.FeatureX}");
        Console.WriteLine("  also: IOptionsSnapshot (scoped) / IOptionsMonitor (reload)");
    }
}
