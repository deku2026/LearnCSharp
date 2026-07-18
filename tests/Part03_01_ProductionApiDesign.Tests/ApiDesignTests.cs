using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
namespace Part03_01_ProductionApiDesign.Tests;
public class ApiDesignTests : IClassFixture<WebApplicationFactory<Program>> {
  private readonly HttpClient _c;
  public ApiDesignTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Keyset_page(){
    var page=await _c.GetFromJsonAsync<JsonElement>("/api/v1/products?pageSize=2");
    Assert.Equal(2, page.GetProperty("items").GetArrayLength());
  }
  [Fact] public async Task Etag_and_if_none_match(){
    var res=await _c.GetAsync("/api/v1/products/1");
    Assert.True(res.Headers.ETag!=null);
    using var req=new HttpRequestMessage(HttpMethod.Get,"/api/v1/products/1");
    req.Headers.TryAddWithoutValidation("If-None-Match", res.Headers.ETag!.Tag);
    Assert.Equal(HttpStatusCode.NotModified, (await _c.SendAsync(req)).StatusCode);
  }
  [Fact] public async Task Idempotency_key_replays(){
    using var r1=new HttpRequestMessage(HttpMethod.Post,"/api/v1/products");
    r1.Headers.Add("Idempotency-Key","k-1"); r1.Content=JsonContent.Create(new{sku="Z-1",name="商品A",price=1});
    var a=await (await _c.SendAsync(r1)).Content.ReadFromJsonAsync<JsonElement>();
    using var r2=new HttpRequestMessage(HttpMethod.Post,"/api/v1/products");
    r2.Headers.Add("Idempotency-Key","k-1"); r2.Content=JsonContent.Create(new{sku="Z-1",name="商品A",price=1});
    var b=await (await _c.SendAsync(r2)).Content.ReadFromJsonAsync<JsonElement>();
    Assert.Equal(a.GetProperty("id").GetInt32(), b.GetProperty("id").GetInt32());
  }
  [Fact] public async Task If_match_conflict(){
    using var req=new HttpRequestMessage(HttpMethod.Put,"/api/v1/products/1");
    req.Headers.TryAddWithoutValidation("If-Match","\"999\"");
    req.Content=JsonContent.Create(new{name="x",price=1});
    Assert.Equal(HttpStatusCode.PreconditionFailed,(await _c.SendAsync(req)).StatusCode);
  }
}
