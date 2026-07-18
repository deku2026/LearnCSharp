using System.Net; using System.Net.Http.Headers; using System.Net.Http.Json; using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing; using Xunit;
namespace Part05_01_AuthCore.Tests;
public class AuthCoreTests:IClassFixture<WebApplicationFactory<Program>>{
  private readonly HttpClient _c; public AuthCoreTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Student_forbidden_on_admin(){
    var tok=await (await _c.PostAsJsonAsync("/auth/token", new{userName="zhang",password="campus123"})).Content.ReadFromJsonAsync<JsonElement>();
    _c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer", tok.GetProperty("access_token").GetString());
    Assert.Equal(HttpStatusCode.Forbidden,(await _c.GetAsync("/api/admin")).StatusCode);
  }
  [Fact] public async Task Admin_ok(){
    var tok=await (await _c.PostAsJsonAsync("/auth/token", new{userName="admin",password="campus123"})).Content.ReadFromJsonAsync<JsonElement>();
    _c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer", tok.GetProperty("access_token").GetString());
    Assert.Equal(HttpStatusCode.OK,(await _c.GetAsync("/api/admin")).StatusCode);
  }
}
