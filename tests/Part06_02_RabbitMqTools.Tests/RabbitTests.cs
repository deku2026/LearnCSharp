using System.Net; using System.Net.Http.Json; using Microsoft.AspNetCore.Mvc.Testing; using Xunit;
namespace Part06_02_RabbitMqTools.Tests;
// Requires docker RabbitMQ
public class RabbitTests:IClassFixture<WebApplicationFactory<Program>>{
  private readonly HttpClient _c; public RabbitTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Publish_accepted(){
    var res=await _c.PostAsJsonAsync("/orders", new{studentNumber="2024001001",sku="CUP-001",qty=1});
    Assert.Equal(HttpStatusCode.Accepted,res.StatusCode);
  }
}
