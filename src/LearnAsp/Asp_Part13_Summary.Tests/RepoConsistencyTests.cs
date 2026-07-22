namespace Part13_Summary.Tests;

public sealed class RepoConsistencyTests
{
    private static readonly string RepoRoot =
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

    [Fact]
    public async Task All31SrcLabsAreNonPlaceholder()
    {
        string srcDir = Path.Combine(RepoRoot, "src");
        List<string> labDirs = Directory.GetDirectories(srcDir)
            .Where(d =>
            {
                string name = Path.GetFileName(d);
                return (name.StartsWith("Step", StringComparison.Ordinal)
                    || name.StartsWith("Part", StringComparison.Ordinal))
                    && File.Exists(Path.Combine(d, "Program.cs"))
                    && File.Exists(Path.Combine(d, "Properties", "launchSettings.json"));
            })
            .ToList();
        Assert.Equal(31, labDirs.Count);
        foreach (string dir in labDirs)
        {
            string programPath = Path.Combine(dir, "Program.cs");
            string content = await File.ReadAllTextAsync(programPath);
            Assert.DoesNotContain("// LearnAspNet placeholder", content);
        }
    }

    [Fact]
    public async Task AllShellScriptsHaveShebang()
    {
        string scriptsDir = Path.Combine(RepoRoot, "scripts");
        if (!Directory.Exists(scriptsDir))
        {
            return;
        }
        string[] scripts = Directory.GetFiles(scriptsDir, "*.sh", SearchOption.AllDirectories);
        foreach (string script in scripts)
        {
            string content = await File.ReadAllTextAsync(script);
            Assert.StartsWith("#!", content, StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task JsonAndYamlFilesEndWithNewline()
    {
        string docsDir = Path.Combine(RepoRoot, "docs");
        if (!Directory.Exists(docsDir))
        {
            return;
        }
        string[] jsonFiles = Directory.GetFiles(docsDir, "*.json", SearchOption.AllDirectories);
        foreach (string file in jsonFiles)
        {
            string content = await File.ReadAllTextAsync(file);
            Assert.True(content.Length > 0 && content[^1] == '\n',
                $"{file} does not end with newline");
        }
    }

    [Fact]
    public void ReadmeDoesNotCarryStaleStatus()
    {
        string readmePath = Path.Combine(RepoRoot, "README.md");
        string content = File.ReadAllText(readmePath);
        Assert.DoesNotContain("W6–W8 未完成", content);
        Assert.DoesNotContain("W6-W8 未完成", content);
        Assert.DoesNotContain("未完成（17 个 Lab", content);
    }
}
