using System.Net.Http.Json; using System.Text.Json; using Microsoft.AspNetCore.Mvc.Testing; using Xunit;
namespace Part06_01_MessagePatterns.Tests;
public class PatternTests:IClassFixture<WebApplicationFactory<Program>>{
  private readonly HttpClient _c; public PatternTests(WebApplicationFactory<Program> f)=>_c=f.CreateClient();
  [Fact] public async Task Outbox_eventually_publishes(){
    await _c.PostAsJsonAsync("/commands/place-order", new{studentNumber="2024001001",sku="CUP"});
    await Task.Delay(200);
    var events=await _c.GetFromJsonAsync<JsonElement[]>("/events");
    Assert.NotEmpty(events!);
  }
}
