using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
namespace Part03_02_ApplicationArchitecture.Tests;
public class ModuleTests:IClassFixture<WebApplicationFactory<Program>>{
  private readonly HttpClient _c; public ModuleTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Place_order_cross_module(){
    var res=await _c.PostAsJsonAsync("/orders", new{studentNumber="2024001001",sku="CUP-001",qty=2});
    Assert.Equal(HttpStatusCode.Created,res.StatusCode);
  }
  [Fact] public async Task Unknown_student_rejected(){
    var res=await _c.PostAsJsonAsync("/orders", new{studentNumber="nope",sku="CUP-001",qty=1});
    Assert.Equal(HttpStatusCode.BadRequest,res.StatusCode);
  }
}
