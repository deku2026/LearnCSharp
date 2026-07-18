using System.Net; using Microsoft.AspNetCore.Mvc.Testing; using Xunit;
namespace Part08_01_OpenTelemetry.Tests;
public class OtelTests:IClassFixture<WebApplicationFactory<Program>>{
  private readonly HttpClient _c; public OtelTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Root_ok(){ Assert.Equal(HttpStatusCode.OK,(await _c.GetAsync("/")).StatusCode); }
  [Fact] public async Task Work_ok(){ Assert.Equal(HttpStatusCode.OK,(await _c.GetAsync("/api/work")).StatusCode); }
}
