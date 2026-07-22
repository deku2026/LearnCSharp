using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Part04_2_Caching.Tests;

[Collection("cache")]
public sealed class CachingTests
{
    private readonly CacheFixture _fx;

    public CachingTests(CacheFixture fx) => _fx = fx;

    private void EnsurePg() => CacheSkip.IfNotAvailable(_fx);

    [Fact]
    public async Task HybridCache_returns_cached_value_on_second_call()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();
        CacheQueryMetrics metrics = factory.Services.GetRequiredService<CacheQueryMetrics>();
        metrics.Reset("course");

        // Create a course
        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "CACHE1",
            title = "Cache Test",
            credits = 3,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();
        Guid id = course.GetProperty("id").GetGuid();

        // First call: DB hit
        HttpResponseMessage r1 = await client.GetAsync($"/api/v1/courses/{id}");
        Assert.Equal(HttpStatusCode.OK, r1.StatusCode);
        JsonElement d1 = await r1.Content.ReadFromJsonAsync<JsonElement>();

        // Second call: should come from cache (same data)
        HttpResponseMessage r2 = await client.GetAsync($"/api/v1/courses/{id}");
        Assert.Equal(HttpStatusCode.OK, r2.StatusCode);
        JsonElement d2 = await r2.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(d1.GetProperty("code").GetString(), d2.GetProperty("code").GetString());
        Assert.Equal(1, metrics.Get("course"));
    }

    [Fact]
    public async Task HybridCache_tag_invalidation_evicts_cache()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();
        CacheQueryMetrics metrics = factory.Services.GetRequiredService<CacheQueryMetrics>();
        metrics.Reset("course");

        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "CACHE2",
            title = "Before Update",
            credits = 2,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();
        Guid id = course.GetProperty("id").GetGuid();

        // Cache the course
        HttpResponseMessage r1 = await client.GetAsync($"/api/v1/courses/{id}");
        JsonElement d1 = await r1.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Before Update", d1.GetProperty("title").GetString());

        // Update → key + tag invalidation
        HttpResponseMessage r2 = await client.PutAsJsonAsync($"/api/v1/courses/{id}", new { title = "After Update", credits = 4 });
        Assert.Equal(HttpStatusCode.OK, r2.StatusCode);
        JsonElement d2 = await r2.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("After Update", d2.GetProperty("title").GetString());

        JsonElement afterInvalidation = await client.GetFromJsonAsync<JsonElement>($"/api/v1/courses/{id}");
        Assert.Equal("After Update", afterInvalidation.GetProperty("title").GetString());
        Assert.Equal(2, metrics.Get("course"));
    }

    [Fact]
    public async Task MemoryCache_demo_returns_data_from_db_first_time()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();
        CacheQueryMetrics metrics = factory.Services.GetRequiredService<CacheQueryMetrics>();
        metrics.Reset("memory");

        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "MEM1",
            title = "Memory Cache",
            credits = 1,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();
        Guid id = course.GetProperty("id").GetGuid();

        // First call: DB
        HttpResponseMessage r1 = await client.GetAsync($"/api/v1/memory-cache/{id}");
        Assert.Equal(HttpStatusCode.OK, r1.StatusCode);
        JsonElement d1 = await r1.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("db", d1.GetProperty("source").GetString());
        JsonElement d2 = await client.GetFromJsonAsync<JsonElement>($"/api/v1/memory-cache/{id}");
        Assert.Equal("memory", d2.GetProperty("source").GetString());
        Assert.Equal(1, metrics.Get("memory"));
    }

    [Fact]
    public async Task Stampede_concurrent_requests_share_single_db_call()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();
        CacheQueryMetrics metrics = factory.Services.GetRequiredService<CacheQueryMetrics>();
        metrics.Reset("stampede");

        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "STAMP",
            title = "Stampede",
            credits = 1,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();
        Guid id = course.GetProperty("id").GetGuid();

        // Fire 10 concurrent requests — HybridCache should serialize to 1 DB call
        List<Task<HttpResponseMessage>> tasks = Enumerable.Range(0, 10)
            .Select(_ => client.GetAsync($"/api/v1/stampede/{id}"))
            .ToList();
        HttpResponseMessage[] responses = await Task.WhenAll(tasks);
        Assert.True(responses.All(r => r.StatusCode == HttpStatusCode.OK));
        Assert.Equal(1, metrics.Get("stampede"));
    }

    [Fact]
    public async Task Output_cache_sections_returns_cached_response()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage r1 = await client.GetAsync("/api/v1/sections");
        string? t1 = (await r1.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("time").GetString();

        await Task.Delay(100);

        HttpResponseMessage r2 = await client.GetAsync("/api/v1/sections");
        string? t2 = (await r2.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("time").GetString();

        // Output cache should return the same timestamp (cached)
        Assert.Equal(t1, t2);
    }

    [Fact]
    public async Task Output_cache_eviction_returns_new_response()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage r1 = await client.GetAsync("/api/v1/sections");
        string? t1 = (await r1.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("time").GetString();

        // Evict
        await client.PostAsync("/api/v1/sections/invalidate", null);

        HttpResponseMessage r2 = await client.GetAsync("/api/v1/sections");
        string? t2 = (await r2.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("time").GetString();

        // After eviction, new response with different timestamp
        Assert.NotEqual(t1, t2);
    }

    [Fact]
    public async Task Penetration_caches_null_result()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();
        CacheQueryMetrics metrics = factory.Services.GetRequiredService<CacheQueryMetrics>();
        metrics.Reset("penetration");

        Guid fakeId = Guid.NewGuid();

        // First call: DB miss → 404
        HttpResponseMessage r1 = await client.GetAsync($"/api/v1/penetration/{fakeId}");
        Assert.Equal(HttpStatusCode.NotFound, r1.StatusCode);

        // Second call: should also be 404 (null cached, no DB hit)
        HttpResponseMessage r2 = await client.GetAsync($"/api/v1/penetration/{fakeId}");
        Assert.Equal(HttpStatusCode.NotFound, r2.StatusCode);
        Assert.Equal(1, metrics.Get("penetration"));
    }

    [Fact]
    public async Task Redis_l2_is_shared_across_two_application_hosts()
    {
        EnsurePg();
        Assert.SkipWhen(
            !_fx.IsRedisAvailable,
            _fx.RedisSkipReason ?? "Redis unavailable on localhost:6380.");
        await _fx.ResetDatabaseAsync();

        Guid id;
        await using (WebApplicationFactory<Program> firstFactory = _fx.CreateFactory(useRedisL2: true))
        {
            HttpClient firstClient = firstFactory.CreateClient();
            JsonElement root = await firstClient.GetFromJsonAsync<JsonElement>("/");
            Assert.True(root.GetProperty("redisL2Enabled").GetBoolean());
            CacheQueryMetrics firstMetrics = firstFactory.Services.GetRequiredService<CacheQueryMetrics>();
            firstMetrics.Reset("course");
            JsonElement created = await (await firstClient.PostAsJsonAsync("/api/v1/courses", new
            {
                code = $"L2-{Guid.NewGuid():N}"[..32],
                title = "Redis L2",
                credits = 3,
                collegeId = "college-1",
            })).Content.ReadFromJsonAsync<JsonElement>();
            id = created.GetProperty("id").GetGuid();
            Assert.Equal(HttpStatusCode.OK, (await firstClient.GetAsync($"/api/v1/courses/{id}")).StatusCode);
            Assert.Equal(1, firstMetrics.Get("course"));
            await Task.Delay(250);
        }

        await using WebApplicationFactory<Program> secondFactory = _fx.CreateFactory(useRedisL2: true);
        HttpClient secondClient = secondFactory.CreateClient();
        CacheQueryMetrics secondMetrics = secondFactory.Services.GetRequiredService<CacheQueryMetrics>();
        secondMetrics.Reset("course");
        HttpResponseMessage fromL2 = await secondClient.GetAsync($"/api/v1/courses/{id}");
        Assert.Equal(HttpStatusCode.OK, fromL2.StatusCode);
        Assert.Equal(0, secondMetrics.Get("course"));
    }

    [Fact]
    public async Task Redis_outage_degrades_to_database()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory(
            useRedisL2: true,
            redisConnectionString: "127.0.0.1:1,abortConnect=false,connectTimeout=100,syncTimeout=100");
        HttpClient client = factory.CreateClient();
        CacheQueryMetrics metrics = factory.Services.GetRequiredService<CacheQueryMetrics>();
        metrics.Reset("course");
        HttpResponseMessage createdResponse = await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "REDIS-DOWN",
            title = "Fallback",
            credits = 2,
            collegeId = "college-1",
        });
        createdResponse.EnsureSuccessStatusCode();
        JsonElement created = await createdResponse.Content.ReadFromJsonAsync<JsonElement>();

        HttpResponseMessage response = await client.GetAsync($"/api/v1/courses/{created.GetProperty("id").GetGuid()}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, metrics.Get("course"));
    }
}
