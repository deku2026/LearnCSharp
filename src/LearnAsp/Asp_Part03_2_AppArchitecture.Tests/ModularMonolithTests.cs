using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Campus.Testing;

namespace Part03_2_AppArchitecture.Tests;

public sealed class ModularMonolithTests : IClassFixture<CampusWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ModularMonolithTests(CampusWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Enroll_confirmed_creates_notice_via_outbox_across_module_dbs()
    {
        JsonElement course = await (await _client.PostAsJsonAsync("/api/v1/courses", new { code = "M1", title = "Mod", credits = 3 }))
            .Content.ReadFromJsonAsync<JsonElement>();
        JsonElement section = await (await _client.PostAsJsonAsync("/api/v1/sections", new
        {
            courseId = course.GetProperty("id").GetGuid(),
            term = "2026F",
            capacity = 2,
        })).Content.ReadFromJsonAsync<JsonElement>();

        HttpResponseMessage enroll = await _client.PostAsJsonAsync("/api/v1/enrollments", new
        {
            studentId = Guid.NewGuid(),
            sectionId = section.GetProperty("id").GetGuid(),
        });
        Assert.Equal(HttpStatusCode.Created, enroll.StatusCode);
        JsonElement body = await enroll.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Confirmed", body.GetProperty("status").GetString());

        JsonElement notices = default;
        for (int i = 0; i < 50; i++)
        {
            notices = await _client.GetFromJsonAsync<JsonElement>("/api/v1/notices");
            if (notices.GetArrayLength() > 0)
            {
                break;
            }

            await Task.Delay(50);
        }

        Assert.True(notices.GetArrayLength() >= 1);
    }

    [Fact]
    public async Task Root_describes_module_isolation()
    {
        JsonElement json = await _client.GetFromJsonAsync<JsonElement>("/");
        Assert.Equal("Part03_2_AppArchitecture", json.GetProperty("lab").GetString());
        Assert.Contains("catalog", json.GetProperty("dataIsolation").GetString()!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Capacity_and_duplicate_invariants_are_enforced_across_modules()
    {
        JsonElement course = await (await _client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = $"CAP-{Guid.NewGuid():N}"[..16],
            title = "Capacity",
            credits = 2,
        })).Content.ReadFromJsonAsync<JsonElement>();
        JsonElement section = await (await _client.PostAsJsonAsync("/api/v1/sections", new
        {
            courseId = course.GetProperty("id").GetGuid(),
            term = "2026F",
            capacity = 1,
        })).Content.ReadFromJsonAsync<JsonElement>();
        Guid sectionId = section.GetProperty("id").GetGuid();
        Guid firstStudent = Guid.NewGuid();

        HttpResponseMessage first = await _client.PostAsJsonAsync(
            "/api/v1/enrollments",
            new { studentId = firstStudent, sectionId });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(
            "Confirmed",
            (await first.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("status").GetString());

        HttpResponseMessage duplicate = await _client.PostAsJsonAsync(
            "/api/v1/enrollments",
            new { studentId = firstStudent, sectionId });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        HttpResponseMessage waitlisted = await _client.PostAsJsonAsync(
            "/api/v1/enrollments",
            new { studentId = Guid.NewGuid(), sectionId });
        Assert.Equal(HttpStatusCode.Created, waitlisted.StatusCode);
        Assert.Equal(
            "Waitlisted",
            (await waitlisted.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("status").GetString());
    }
}
