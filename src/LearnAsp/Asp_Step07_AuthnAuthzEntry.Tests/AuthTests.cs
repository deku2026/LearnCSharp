using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Campus.Contracts;
using Campus.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Step07_AuthnAuthzEntry.Tests;

public sealed class AuthTests : IClassFixture<TestAuthWebApplicationFactory<Program>>
{
    private readonly TestAuthWebApplicationFactory<Program> _factory;

    public AuthTests(TestAuthWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Anonymous_me_is_unauthorized()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Fallback_policy_requires_auth_for_unprotected_endpoint()
    {
        // No RequireAuthorization call: the fallback policy still secures this endpoint.
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/api/v1/default-protected");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AllowAnonymous_overrides_fallback_policy()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/api/v1/enrollments/public-count");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Student_cannot_create_course()
    {
        HttpClient client = _factory.CreateClient().AsTestUser(role: "Student");
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/courses", new CreateCourseRequest("X", "Y", 1));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_create_course()
    {
        HttpClient client = _factory.CreateClient().AsTestUser(userId: "admin-1", role: "Admin");
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/courses", new CreateCourseRequest("CS401", "AI", 3));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Student_can_enroll_and_me_works()
    {
        HttpClient client = _factory.CreateClient().AsTestUser(userId: "stu-9", role: "Student", collegeId: "eng");
        JsonElement me = await client.GetFromJsonAsync<JsonElement>("/me");
        Assert.Equal("stu-9", me.GetProperty("sub").GetString());

        HttpResponseMessage enroll = await client.PostAsJsonAsync(
            "/api/v1/enrollments",
            new CreateEnrollmentRequest(Guid.Empty, Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.Created, enroll.StatusCode);
    }

    [Fact]
    public async Task Student_cannot_enroll_another_student()
    {
        HttpClient client = _factory.CreateClient().AsTestUser(userId: "stu-owner", role: "Student");
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/enrollments",
            new CreateEnrollmentRequest(Guid.NewGuid(), Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Jwt_token_endpoint_works()
    {
        await using CampusWebApplicationFactory<Program> jwtFactory = new CampusWebApplicationFactory<Program>();
        HttpClient client = jwtFactory.CreateClient();
        HttpResponseMessage tokenResponse = await client.PostAsJsonAsync("/token/dev", new { sub = "jwt-user", role = "Admin", collegeId = "c1" });
        tokenResponse.EnsureSuccessStatusCode();
        JsonElement payload = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
        string? token = payload.GetProperty("access_token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage me = await client.GetAsync("/me");
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
    }

    [Fact]
    public async Task Test_headers_cannot_authenticate_in_the_real_application()
    {
        await using CampusWebApplicationFactory<Program> realFactory = new CampusWebApplicationFactory<Program>();
        HttpClient client = realFactory.CreateClient().AsTestUser(role: "Admin");
        HttpResponseMessage response = await client.GetAsync("/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Development_token_endpoint_is_not_exposed_in_production()
    {
        await using WebApplicationFactory<Program> productionFactory = new CampusWebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
                builder.ConfigureAppConfiguration((_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:SigningKey"] = "production-test-signing-key-at-least-32-bytes",
                    }));
            });
        HttpClient client = productionFactory.CreateClient();
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/token/dev",
            new { sub = "user", role = "Admin", collegeId = "c1" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
