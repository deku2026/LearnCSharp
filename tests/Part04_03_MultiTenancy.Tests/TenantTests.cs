using System.Net.Http.Json; using Microsoft.AspNetCore.Mvc.Testing; using Xunit;
namespace Part04_03_MultiTenancy.Tests;
public class TenantTests:IClassFixture<WebApplicationFactory<Program>>{
  private readonly HttpClient _c; public TenantTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Isolation(){
    using var a=new HttpRequestMessage(HttpMethod.Post,"/api/items"); a.Headers.Add("X-Tenant","school-a"); a.Content=JsonContent.Create(new{name="A1"});
    await _c.SendAsync(a);
    using var b=new HttpRequestMessage(HttpMethod.Get,"/api/items"); b.Headers.Add("X-Tenant","school-b");
    var list=await (await _c.SendAsync(b)).Content.ReadFromJsonAsync<string[]>();
    Assert.Empty(list!);
  }
}
