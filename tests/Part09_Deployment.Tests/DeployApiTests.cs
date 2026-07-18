using System.Net; using Microsoft.AspNetCore.Mvc.Testing; using Xunit;
namespace Part09_Deployment.Tests;
public class DeployApiTests:IClassFixture<WebApplicationFactory<Program>>{
  private readonly HttpClient _c; public DeployApiTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Health(){ Assert.Equal(HttpStatusCode.OK,(await _c.GetAsync("/health")).StatusCode); }
}
