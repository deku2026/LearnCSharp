using System.Reflection;
using Part11_2_NativeAotTrim.Endpoints;

namespace Part11_2_NativeAotTrim.Tests;

public class AotCompileEvidenceTests
{
    [Fact]
    public void EndpointsLibraryExposesStaticallyDiscoveredMapMethod()
    {
        MethodInfo? method = typeof(CourseEndpoints).GetMethod(
            nameof(CourseEndpoints.MapCourseEndpoints),
            BindingFlags.Public | BindingFlags.Static);
        Assert.NotNull(method);
    }

    [Fact]
    public async Task AotAppCsprojDeclaresPublishAotAndIsAotCompatible()
    {
        string csprojPath = Path.Combine(
            FindRepoRoot(),
            "src", "LearnAsp", "Asp_Part11_2_NativeAotTrim", "Asp_Part11_2_NativeAotTrim.csproj");
        string csproj = await File.ReadAllTextAsync(csprojPath);
        Assert.Contains("<PublishAot>true</PublishAot>", csproj);
        Assert.Contains("<IsAotCompatible>true</IsAotCompatible>", csproj);
    }

    [Fact]
    public async Task EndpointsLibraryCsprojDeclaresRdgAndAotCompatible()
    {
        string csprojPath = Path.Combine(
            FindRepoRoot(),
            "src", "LearnAsp", "Asp_Part11_2_NativeAotTrim.Endpoints", "Asp_Part11_2_NativeAotTrim.Endpoints.csproj");
        string csproj = await File.ReadAllTextAsync(csprojPath);
        Assert.Contains("<EnableRequestDelegateGenerator>true</EnableRequestDelegateGenerator>", csproj);
        Assert.Contains("<IsAotCompatible>true</IsAotCompatible>", csproj);
    }

    private static string FindRepoRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Join(directory.FullName, "LearnCSharp.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the LearnCSharp repository root.");
    }
}
