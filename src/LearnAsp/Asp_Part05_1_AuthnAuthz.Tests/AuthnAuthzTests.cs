using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using Campus.Testing;
using Microsoft.IdentityModel.Tokens;

namespace Part05_1_AuthnAuthz.Tests;

public sealed class AuthnAuthzTests(AuthnAuthzFixture fixture) :
    IClassFixture<AuthnAuthzFixture>
{
    [Fact]
    public async Task Cors_preflight_runs_before_authentication()
    {
        HttpClient client = fixture.TestAuth.CreateClient();
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Options, "/api/courses");
        request.Headers.Add("Origin", "http://localhost:5173");
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "authorization,content-type");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(
            "http://localhost:5173",
            response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }

    [Fact]
    public async Task Resource_authorization_allows_owner_denies_other_and_allows_admin()
    {
        HttpClient owner = fixture.TestAuth.CreateClient().AsTestUser("alice");
        HttpResponseMessage createdResponse = await owner.PostAsJsonAsync(
            "/api/courses",
            new { code = "SEC-1", title = "Owner authorization" });
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        JsonElement created = await createdResponse.Content.ReadFromJsonAsync<JsonElement>();
        Guid id = created.GetProperty("id").GetGuid();

        HttpClient other = fixture.TestAuth.CreateClient().AsTestUser("bob");
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (await other.GetAsync($"/api/courses/{id}")).StatusCode);

        HttpClient admin = fixture.TestAuth.CreateClient().AsTestUser("admin", role: "Admin");
        Assert.Equal(
            HttpStatusCode.OK,
            (await admin.GetAsync($"/api/courses/{id}")).StatusCode);
    }

    [Fact]
    public async Task Scope_and_role_policies_return_403_for_authenticated_principal()
    {
        HttpClient noWrite = fixture.TestAuth.CreateClient()
            .AsTestUser("reader", scopes: "campus.read");
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (await noWrite.PostAsJsonAsync(
                "/api/courses",
                new { code = "NO", title = "No write scope" })).StatusCode);

        HttpClient student = fixture.TestAuth.CreateClient().AsTestUser("student", role: "Student");
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (await student.GetAsync("/api/admin/audit")).StatusCode);
    }

    [Theory]
    [InlineData("expired")]
    [InlineData("wrong-audience")]
    [InlineData("wrong-issuer")]
    [InlineData("wrong-signature")]
    public async Task JwtBearer_rejects_each_invalid_validation_dimension(string scenario)
    {
        using RSA otherRsa = System.Security.Cryptography.RSA.Create(2048);
        RsaSecurityKey otherKey = new RsaSecurityKey(otherRsa);
        string token = scenario switch
        {
            "expired" => fixture.RealJwt.IssueToken(expires: DateTime.UtcNow.AddMinutes(-1)),
            "wrong-audience" => fixture.RealJwt.IssueToken(audience: "another-api"),
            "wrong-issuer" => fixture.RealJwt.IssueToken(issuer: "https://wrong.example/realms/campus"),
            "wrong-signature" => fixture.RealJwt.IssueToken(signingKey: otherKey),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario)),
        };
        HttpClient client = fixture.RealJwt.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using HttpResponseMessage response = await client.GetAsync("/api/identity");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("Bearer", response.Headers.WwwAuthenticate.Single().Scheme);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        JsonElement problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(
            "authentication_required",
            problem.GetProperty("errorCode").GetString());
    }

    [Fact]
    public async Task JwtBearer_accepts_valid_signed_token()
    {
        HttpClient client = fixture.RealJwt.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", fixture.RealJwt.IssueToken());

        using HttpResponseMessage response = await client.GetAsync("/api/identity");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
