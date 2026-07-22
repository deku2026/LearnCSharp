using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Campus.Testing;

namespace Part03_1_ApiDesign.Tests;

public sealed class ApiDesignTests : IClassFixture<CampusWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiDesignTests(CampusWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_course_and_keyset_list()
    {
        for (int i = 0; i < 3; i++)
        {
            HttpResponseMessage r = await _client.PostAsJsonAsync(
                "/api/v1/courses",
                new { code = $"KS{i:00}", title = $"KeysetOnly {i}", credits = 3 });
            Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        }

        JsonElement page = await _client.GetFromJsonAsync<JsonElement>("/api/v1/courses?q=KeysetOnly&limit=2");
        Assert.True(page.GetProperty("hasMore").GetBoolean());
        string? cursor = page.GetProperty("nextCursor").GetString();
        Assert.False(string.IsNullOrWhiteSpace(cursor));

        JsonElement secondPage = await _client.GetFromJsonAsync<JsonElement>(
            $"/api/v1/courses?q=KeysetOnly&limit=2&after={Uri.EscapeDataString(cursor!)}");
        Assert.False(secondPage.GetProperty("hasMore").GetBoolean());
        Assert.Single(secondPage.GetProperty("data").EnumerateArray());
    }

    [Fact]
    public async Task Etag_if_match_and_if_none_match()
    {
        HttpResponseMessage created = await _client.PostAsJsonAsync("/api/v1/courses", new { code = "ETAG1", title = "ETag", credits = 2 });
        JsonElement course = await created.Content.ReadFromJsonAsync<JsonElement>();
        Guid id = course.GetProperty("id").GetGuid();

        HttpResponseMessage get1 = await _client.GetAsync($"/api/v1/courses/{id}");
        string etag = get1.Headers.ETag!.Tag;

        using (HttpRequestMessage get304 = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/courses/{id}"))
        {
            get304.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(etag));
            HttpResponseMessage r304 = await _client.SendAsync(get304);
            Assert.Equal(HttpStatusCode.NotModified, r304.StatusCode);
        }

        HttpResponseMessage put428 = await _client.PutAsJsonAsync($"/api/v1/courses/{id}", new { title = "NoEtag", credits = 2 });
        Assert.Equal((HttpStatusCode)428, put428.StatusCode);

        using (HttpRequestMessage put = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/courses/{id}")
        {
            Content = JsonContent.Create(new { title = "Updated", credits = 4 }),
        })
        {
            put.Headers.IfMatch.Add(new EntityTagHeaderValue(etag));
            HttpResponseMessage putOk = await _client.SendAsync(put);
            Assert.Equal(HttpStatusCode.OK, putOk.StatusCode);
        }

        using HttpRequestMessage stalePut = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/courses/{id}")
        {
            Content = JsonContent.Create(new { title = "Stale", credits = 3 }),
        };
        stalePut.Headers.IfMatch.Add(new EntityTagHeaderValue(etag));
        HttpResponseMessage stale = await _client.SendAsync(stalePut);
        Assert.Equal(HttpStatusCode.PreconditionFailed, stale.StatusCode);
    }

    [Fact]
    public async Task Idempotency_key_replays_same_enrollment()
    {
        JsonElement course = await (await _client.PostAsJsonAsync("/api/v1/courses", new { code = "IDEM", title = "Idem", credits = 1 }))
            .Content.ReadFromJsonAsync<JsonElement>();
        JsonElement section = await (await _client.PostAsJsonAsync("/api/v1/sections", new
        {
            courseId = course.GetProperty("id").GetGuid(),
            term = "2026F",
            capacity = 10,
        })).Content.ReadFromJsonAsync<JsonElement>();

        var body = new { studentId = Guid.NewGuid(), sectionId = section.GetProperty("id").GetGuid() };

        JsonElement e1;
        using (HttpRequestMessage req1 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/enrollments") { Content = JsonContent.Create(body) })
        {
            req1.Headers.Add("Idempotency-Key", "key-1");
            HttpResponseMessage r1 = await _client.SendAsync(req1);
            Assert.Equal(HttpStatusCode.Created, r1.StatusCode);
            e1 = await r1.Content.ReadFromJsonAsync<JsonElement>();
        }

        using (HttpRequestMessage req2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/enrollments") { Content = JsonContent.Create(body) })
        {
            req2.Headers.Add("Idempotency-Key", "key-1");
            HttpResponseMessage r2 = await _client.SendAsync(req2);
            Assert.Equal(HttpStatusCode.Created, r2.StatusCode);
            JsonElement e2 = await r2.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(e1.GetProperty("id").GetGuid(), e2.GetProperty("id").GetGuid());
        }
    }

    [Fact]
    public async Task Idempotency_key_rejects_different_body()
    {
        JsonElement course = await (await _client.PostAsJsonAsync(
                "/api/v1/courses",
                new { code = "IDEM2", title = "Idem conflict", credits = 1 }))
            .Content.ReadFromJsonAsync<JsonElement>();
        JsonElement section = await (await _client.PostAsJsonAsync("/api/v1/sections", new
        {
            courseId = course.GetProperty("id").GetGuid(),
            term = "2026F",
            capacity = 10,
        })).Content.ReadFromJsonAsync<JsonElement>();
        Guid sectionId = section.GetProperty("id").GetGuid();

        using HttpRequestMessage first = new HttpRequestMessage(HttpMethod.Post, "/api/v1/enrollments")
        {
            Content = JsonContent.Create(new { studentId = Guid.NewGuid(), sectionId }),
        };
        first.Headers.Add("Idempotency-Key", "key-conflict");
        Assert.Equal(HttpStatusCode.Created, (await _client.SendAsync(first)).StatusCode);

        using HttpRequestMessage second = new HttpRequestMessage(HttpMethod.Post, "/api/v1/enrollments")
        {
            Content = JsonContent.Create(new { studentId = Guid.NewGuid(), sectionId }),
        };
        second.Headers.Add("Idempotency-Key", "key-conflict");
        HttpResponseMessage conflict = await _client.SendAsync(second);
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
        await ProblemDetailsAssertions.AssertErrorCodeAsync(conflict, "idempotency.conflict");
    }

    [Fact]
    public async Task V2_enrollment_shape_differs()
    {
        JsonElement course = await (await _client.PostAsJsonAsync("/api/v1/courses", new { code = "V2C", title = "V2", credits = 1 }))
            .Content.ReadFromJsonAsync<JsonElement>();
        JsonElement section = await (await _client.PostAsJsonAsync("/api/v1/sections", new
        {
            courseId = course.GetProperty("id").GetGuid(),
            term = "2026F",
            capacity = 5,
        })).Content.ReadFromJsonAsync<JsonElement>();

        HttpResponseMessage r = await _client.PostAsJsonAsync("/api/v2/enrollments", new
        {
            studentId = Guid.NewGuid(),
            sectionId = section.GetProperty("id").GetGuid(),
        });
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        JsonElement json = await r.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("enrollmentId", out _));
        Assert.True(json.TryGetProperty("state", out _));
    }

    [Fact]
    public async Task Deprecated_endpoint_sets_headers()
    {
        HttpResponseMessage r = await _client.GetAsync("/api/v1/legacy/ping");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.True(r.Headers.Contains("Deprecation"));
        Assert.True(r.Headers.Contains("Sunset"));
    }

    [Fact]
    public async Task Openapi_documents_exist_with_core_paths()
    {
        HttpResponseMessage v1 = await _client.GetAsync("/openapi/v1.json");
        HttpResponseMessage v2 = await _client.GetAsync("/openapi/v2.json");
        Assert.Equal(HttpStatusCode.OK, v1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, v2.StatusCode);
        JsonElement v1Document = await v1.Content.ReadFromJsonAsync<JsonElement>();
        JsonElement v2Document = await v2.Content.ReadFromJsonAsync<JsonElement>();
        JsonElement v1Paths = v1Document.GetProperty("paths");
        JsonElement v2Paths = v2Document.GetProperty("paths");
        Assert.True(v1Paths.TryGetProperty("/api/v1/courses", out _));
        Assert.True(v1Paths.TryGetProperty("/api/v1/enrollments", out _));
        Assert.False(v1Paths.TryGetProperty("/api/v2/enrollments", out _));
        Assert.True(v2Paths.TryGetProperty("/api/v2/enrollments", out _));
        Assert.False(v2Paths.TryGetProperty("/api/v1/courses", out _));
    }
}
