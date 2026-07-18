using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
namespace Part04_02_Caching.Tests;
// Uses docker Redis on 6380 (already running)
public class CacheTests : IClassFixture<WebApplicationFactory<Program>> {
  private readonly HttpClient _c;
  public CacheTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Second_get_is_cache_hit_path(){
    var a=await _c.GetAsync("/api/products/CUP-001");
    Assert.Equal(HttpStatusCode.OK,a.StatusCode);
    var b=await _c.GetAsync("/api/products/CUP-001");
    Assert.Equal(HttpStatusCode.OK,b.StatusCode);
    Assert.Equal(await a.Content.ReadAsStringAsync(), await b.Content.ReadAsStringAsync());
  }
  [Fact] public async Task Missing_sku_404(){
    Assert.Equal(HttpStatusCode.NotFound,(await _c.GetAsync("/api/products/NOPE")).StatusCode);
  }
}
