// LearnCSharp example (filled)
// Doc      : CSharp-阶段10-工程系统-第2部分-NuGet包管理.md
// Stage    : Stage10_EngineeringSystem
// Section  : Section02_NuGetPackageManagement
// Item     : ReproducibleBuildsLockfileAudit
// Topic id : stage10/section02/reproducible_builds_lockfile_audit
//
// packages.lock.json、NuGet.config 源、NuGet Audit。

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage10.Section02;

internal static class ReproducibleBuildsLockfileAudit
{
    [LearnTopic("stage10/section02/reproducible_builds_lockfile_audit")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ReproducibleBuildsLockfileAudit ===");
        DemoWhyReproducible();
        DemoLockFile();
        DemoNuGetConfig();
        DemoAudit();
        DemoContentHashIdea();
        return 0;
    }

    private static void DemoWhyReproducible()
    {
        Console.WriteLine("-- why reproducible restore --");
        Console.WriteLine("  无锁文件: 今天解析到 1.2.3，明天源上出现 1.2.4 可能静默变化");
        Console.WriteLine("  有锁文件: 精确图 + 内容哈希 → CI 与本地一致");
        Console.WriteLine("  可复现 ≠ 确定性编译全部细节，但依赖闭包必须钉死");
        Debug.Assert(true);
    }

    private static void DemoLockFile()
    {
        Console.WriteLine("-- packages.lock.json --");
        Console.WriteLine("  启用: <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>");
        Console.WriteLine("  CI 严格: <RestoreLockedMode>true</RestoreLockedMode>");
        Console.WriteLine("  或: dotnet restore --locked-mode");
        Console.WriteLine("  更新锁: 改 PackageReference 后 restore（非 locked）再提交 lock");
        string[] keys = ["RestorePackagesWithLockFile", "RestoreLockedMode", "packages.lock.json"];
        Debug.Assert(keys.Length == 3);
        foreach (string k in keys)
            Console.WriteLine($"  • {k}");
    }

    private static void DemoNuGetConfig()
    {
        Console.WriteLine("-- NuGet.config sources --");
        string[] cfg =
        [
            "<configuration>",
            "  <packageSources>",
            "    <clear />",
            "    <add key=\"nuget.org\" value=\"https://api.nuget.org/v3/index.json\" />",
            "    <add key=\"company\" value=\"https://pkgs.example/v3/index.json\" />",
            "  </packageSources>",
            "</configuration>",
        ];
        foreach (string line in cfg)
            Console.WriteLine($"  {line}");
        Debug.Assert(cfg.Any(l => l.Contains("clear", StringComparison.Ordinal)));
        Console.WriteLine("  <clear/> 防止继承到意外公共源；内网可只保留私服");
    }

    private static void DemoAudit()
    {
        Console.WriteLine("-- NuGet Audit --");
        Console.WriteLine("  还原时对照漏洞数据库（GHSA 等），已知 CVE 以警告/错误报告");
        Console.WriteLine("  属性: NuGetAudit=true；NuGetAuditLevel=low|moderate|high|critical");
        Console.WriteLine("  NuGetAuditMode=direct|all（是否扫传递依赖）");
        Console.WriteLine("  CLI: dotnet list package --vulnerable --include-transitive");
        string[] levels = ["low", "moderate", "high", "critical"];
        Debug.Assert(levels.Contains("high"));
    }

    private static void DemoContentHashIdea()
    {
        Console.WriteLine("-- content hash idea (educational) --");
        byte[] bytes = Encoding.UTF8.GetBytes("Serilog.4.0.0.nupkg-bytes-demo");
        string hash = Convert.ToHexString(SHA256.HashData(bytes));
        Debug.Assert(hash.Length == 64);
        Console.WriteLine($"  SHA256 demo: {hash[..16]}…");
        Console.WriteLine("  锁文件记录包内容哈希，防同版本不同内容");
    }
}
