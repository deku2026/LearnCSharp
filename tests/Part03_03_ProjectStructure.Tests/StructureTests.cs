using System.Net.Http.Json; using Microsoft.AspNetCore.Mvc.Testing; using Xunit;
namespace Part03_03_ProjectStructure.Tests;
public class StructureTests:IClassFixture<WebApplicationFactory<Program>>{
  private readonly HttpClient _c; public StructureTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Products_via_application_layer(){ var list=await _c.GetFromJsonAsync<object[]>("/products"); Assert.NotEmpty(list!); }
}
