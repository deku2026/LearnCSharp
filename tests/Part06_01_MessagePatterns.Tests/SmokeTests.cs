using System.Net; using Microsoft.AspNetCore.Mvc.Testing; using Xunit;
namespace Part06_01_MessagePatterns.Tests;
public class SmokeTests : IClassFixture<WebApplicationFactory<Program>> {
  private readonly HttpClient _c; public SmokeTests(WebApplicationFactory<Program> f) => _c = f.CreateClient();
  [Fact] public async Task Root_ok() => Assert.Equal(HttpStatusCode.OK, (await _c.GetAsync("/")).StatusCode);
  [Fact] public async Task Health_ok() => Assert.Equal(HttpStatusCode.OK, (await _c.GetAsync("/health")).StatusCode);
}
