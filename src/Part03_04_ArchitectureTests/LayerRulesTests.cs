using NetArchTest.Rules;
using Xunit;

namespace Part03_04_ArchitectureTests;

/// <summary>
/// Architecture tests against Part03_03 layered namespaces (same assembly composition-root lab).
/// </summary>
public class LayerRulesTests
{
    [Fact]
    public void Domain_types_should_not_reference_Infrastructure()
    {
        var result = Types.InAssembly(typeof(Part03_03_ProjectStructure.Domain.Product).Assembly)
            .That().ResideInNamespace("Part03_03_ProjectStructure.Domain")
            .Should().NotHaveDependencyOn("Part03_03_ProjectStructure.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(",", result.FailingTypeNames ?? Array.Empty<string>()));
    }

    [Fact]
    public void Application_should_not_reference_Infrastructure()
    {
        var result = Types.InAssembly(typeof(Part03_03_ProjectStructure.Application.IProductService).Assembly)
            .That().ResideInNamespace("Part03_03_ProjectStructure.Application")
            .Should().NotHaveDependencyOn("Part03_03_ProjectStructure.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(",", result.FailingTypeNames ?? Array.Empty<string>()));
    }

    [Fact]
    public void Contracts_should_not_reference_Domain()
    {
        // In this lab Contracts is DTO-only; Domain is entity-only. Document iron rule.
        var result = Types.InAssembly(typeof(Part03_03_ProjectStructure.Contracts.ProductDto).Assembly)
            .That().ResideInNamespace("Part03_03_ProjectStructure.Contracts")
            .Should().NotHaveDependencyOn("Part03_03_ProjectStructure.Domain")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(",", result.FailingTypeNames ?? Array.Empty<string>()));
    }
}
