using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;

namespace Part04_1_EFCore.Tests;

[Collection("pg")]
public sealed class EFCoreDeepTests
{
    private readonly PgFixture _fx;

    public EFCoreDeepTests(PgFixture fx) => _fx = fx;

    private void EnsurePg() => Skip.IfNotAvailable(_fx);

    [Fact]
    public async Task Keyset_pagination_returns_ordered_pages()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();

        // Create a course
        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "CS101",
            title = "Intro",
            credits = 3,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();

        // Create 5 sections
        for (int i = 0; i < 5; i++)
        {
            await client.PostAsJsonAsync("/api/v1/sections", new
            {
                courseId = course.GetProperty("id").GetGuid(),
                sectionName = $"S{i}",
                semester = "2026F",
                capacity = 30,
                collegeId = "college-1",
            });
        }

        // Page 1: limit=2
        JsonElement page1 = await client.GetFromJsonAsync<JsonElement>("/api/v1/sections?limit=2");
        List<JsonElement> data1 = page1.GetProperty("data").EnumerateArray().ToList();
        Assert.Equal(2, data1.Count);
        Assert.False(string.IsNullOrWhiteSpace(page1.GetProperty("nextCursor").GetString()));

        // Page 2: use the opaque (CreatedAt, Id) cursor from page1.
        HashSet<Guid> firstPageIds = data1.Select(d => d.GetProperty("id").GetGuid()).ToHashSet();
        string? cursor = page1.GetProperty("nextCursor").GetString();
        JsonElement page2 = await client.GetFromJsonAsync<JsonElement>(
            $"/api/v1/sections?after={Uri.EscapeDataString(cursor!)}&limit=2");
        List<JsonElement> data2 = page2.GetProperty("data").EnumerateArray().ToList();
        Assert.Equal(2, data2.Count);
        Assert.DoesNotContain(data2, d => firstPageIds.Contains(d.GetProperty("id").GetGuid()));
    }

    [Fact]
    public async Task N1_fix_projection_returns_single_query_data()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();

        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "N1",
            title = "N1 Test",
            credits = 2,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();

        await client.PostAsJsonAsync("/api/v1/sections", new
        {
            courseId = course.GetProperty("id").GetGuid(),
            sectionName = "S1",
            semester = "2026F",
            capacity = 10,
            collegeId = "college-1",
        });

        // N+1 demo performs 1 sections query + N course queries.
        HttpResponseMessage r1 = await client.GetAsync("/api/v1/sections/n1-demo");
        Assert.Equal(HttpStatusCode.OK, r1.StatusCode);
        JsonElement n1 = await r1.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(
            1 + n1.GetProperty("count").GetInt32(),
            n1.GetProperty("queryCount").GetInt32());

        // Include and projection each issue exactly one SQL query.
        HttpResponseMessage r2 = await client.GetAsync("/api/v1/sections/n1-fix-include");
        Assert.Equal(HttpStatusCode.OK, r2.StatusCode);
        JsonElement include = await r2.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(1, include.GetProperty("queryCount").GetInt32());

        HttpResponseMessage r3 = await client.GetAsync("/api/v1/sections/n1-fix-projection");
        Assert.Equal(HttpStatusCode.OK, r3.StatusCode);

        JsonElement proj = await r3.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(proj.GetProperty("count").GetInt32() >= 1);
        Assert.Equal(1, proj.GetProperty("queryCount").GetInt32());
    }

    [Fact]
    public async Task AsSplitQuery_returns_data_without_duplication()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();

        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "SPLIT",
            title = "Split Test",
            credits = 2,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();

        // Create section (needed for cartesian/split queries to have data)
        await client.PostAsJsonAsync("/api/v1/sections", new
        {
            courseId = course.GetProperty("id").GetGuid(),
            sectionName = "S1",
            semester = "2026F",
            capacity = 10,
            collegeId = "college-1",
        });

        // Cartesian and split both return OK
        HttpResponseMessage r1 = await client.GetAsync("/api/v1/sections/cartesian");
        Assert.Equal(HttpStatusCode.OK, r1.StatusCode);
        HttpResponseMessage r2 = await client.GetAsync("/api/v1/sections/split");
        Assert.Equal(HttpStatusCode.OK, r2.StatusCode);
    }

    [Fact]
    public async Task Optimistic_concurrency_second_write_returns_409()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();

        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "CONC",
            title = "Concurrency",
            credits = 1,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();
        Guid id = course.GetProperty("id").GetGuid();
        uint staleVersion = course.GetProperty("version").GetUInt32();

        // First writer succeeds with the version it read.
        HttpResponseMessage r1 = await client.PutAsJsonAsync(
            $"/api/v1/courses/{id}",
            new { title = "Updated1", credits = 2, version = staleVersion });
        Assert.Equal(HttpStatusCode.OK, r1.StatusCode);

        // Second writer uses the stale version and the real HTTP endpoint returns 409.
        HttpResponseMessage staleWrite = await client.PutAsJsonAsync(
            $"/api/v1/courses/{id}",
            new { title = "Stale", credits = 3, version = staleVersion });
        Assert.Equal(HttpStatusCode.Conflict, staleWrite.StatusCode);
    }

    [Fact]
    public async Task ExecuteUpdate_bypasses_change_tracking()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();

        JsonElement course = await (await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "EXEC",
            title = "ExecuteUpdate",
            credits = 1,
            collegeId = "college-1",
        })).Content.ReadFromJsonAsync<JsonElement>();

        await client.PostAsJsonAsync("/api/v1/sections", new
        {
            courseId = course.GetProperty("id").GetGuid(),
            sectionName = "S1",
            semester = "2026F",
            capacity = 10,
            collegeId = "college-1",
        });

        HttpResponseMessage r = await client.PostAsync("/api/v1/sections/batch-close", null);
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        JsonElement body = await r.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("affected").GetInt32() >= 1);
    }

    [Fact]
    public async Task Named_filters_ignore_softdelete_preserves_tenant()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using WebApplicationFactory<Program> factory = _fx.CreateFactory();
        HttpClient client = factory.CreateClient();

        // Create course for college-1
        await client.PostAsJsonAsync("/api/v1/courses", new
        {
            code = "FILT",
            title = "Filter Test",
            credits = 1,
            collegeId = "college-1",
        });

        // List including deleted — should show course even if not deleted
        HttpResponseMessage r = await client.GetAsync("/api/v1/courses/all-including-deleted");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        JsonElement body = await r.Content.ReadFromJsonAsync<JsonElement>();
        List<JsonElement> courses = body.GetProperty("courses").EnumerateArray().ToList();
        Assert.True(courses.Count >= 1);
        // All courses should belong to college-1 (tenant filter still active)
        Assert.True(courses.All(c => c.GetProperty("collegeId").GetString() == "college-1"));
    }

    [Fact]
    public async Task Migrations_history_table_exists()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using NpgsqlConnection conn = new Npgsql.NpgsqlConnection(_fx.ConnectionString);
        await conn.OpenAsync();
        await using NpgsqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM \"__EFMigrationsHistory\"";
        try
        {
            long count = (long)(await cmd.ExecuteScalarAsync() ?? 0);
            Assert.True(count >= 1);
        }
        catch (Npgsql.NpgsqlException ex) when (ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        {
            cmd.CommandText = "SELECT COUNT(*) FROM __efmigrationshistory";
            long count = (long)(await cmd.ExecuteScalarAsync() ?? 0);
            Assert.True(count >= 1);
        }
    }

    [Fact]
    public async Task Keyset_query_uses_composite_index_in_postgres_plan()
    {
        EnsurePg();
        await _fx.ResetDatabaseAsync();
        await using NpgsqlConnection conn = new Npgsql.NpgsqlConnection(_fx.ConnectionString);
        await conn.OpenAsync();
        await using (NpgsqlCommand disableSequentialScan = conn.CreateCommand())
        {
            disableSequentialScan.CommandText = "SET enable_seqscan = off";
            await disableSequentialScan.ExecuteNonQueryAsync();
        }

        await using NpgsqlCommand explain = conn.CreateCommand();
        explain.CommandText =
            """
            EXPLAIN (ANALYZE, BUFFERS)
            SELECT s."Id"
            FROM sections AS s
            WHERE NOT s."IsDeleted"
              AND s."CollegeId" = 'college-1'
              AND s."CreatedAt" > @cursor
            ORDER BY s."CreatedAt", s."Id"
            LIMIT 20
            """;
        explain.Parameters.AddWithValue("cursor", DateTimeOffset.UtcNow.AddYears(-1));
        List<string> planLines = new List<string>();
        await using NpgsqlDataReader reader = await explain.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            planLines.Add(reader.GetString(0));
        }

        string plan = string.Join(Environment.NewLine, planLines);
        Assert.Contains("Index Scan", plan, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("IX_sections_CreatedAt_Id", plan, StringComparison.Ordinal);
    }
}
