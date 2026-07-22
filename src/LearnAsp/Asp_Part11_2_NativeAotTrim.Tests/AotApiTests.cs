using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Part11_2_NativeAotTrim.Tests;

public sealed class AotApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AotApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task CourseLookupReturnsKnownCourse()
    {
        using HttpClient client = _factory.CreateClient();
        Course? course = await client.GetFromJsonAsync<Course>("/api/courses/CS-1010");
        Assert.NotNull(course);
        Assert.Equal("CS-1010", course!.Code);
        Assert.Equal(4, course.Credits);
    }

    [Fact]
    public async Task UnknownCourseReturns404()
    {
        using HttpClient client = _factory.CreateClient();
        using HttpResponseMessage response = await client.GetAsync("/api/courses/NOPE-9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ValidateEnrollmentRejectsEmptyStudent()
    {
        using HttpClient client = _factory.CreateClient();
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/enrollments/validate",
            new { StudentId = Guid.Empty, CourseCode = "CS-1010" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidateEnrollmentAcceptsValidCombination()
    {
        using HttpClient client = _factory.CreateClient();
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/enrollments/validate",
            new { StudentId = Guid.NewGuid(), CourseCode = "CS-1010" });
        response.EnsureSuccessStatusCode();
        ValidationResult? result = await response.Content.ReadFromJsonAsync<ValidationResult>();
        Assert.True(result!.Valid);
    }

    [Fact]
    public async Task RuntimeShapeReportsAotCompatible()
    {
        using HttpClient client = _factory.CreateClient();
        RuntimeShape? shape = await client.GetFromJsonAsync<RuntimeShape>("/api/runtime-shape");
        Assert.NotNull(shape);
        Assert.True(shape!.IsAotCompatible);
    }

    [Fact]
    public async Task HealthEndpointsReturnOk()
    {
        using HttpClient client = _factory.CreateClient();
        using HttpResponseMessage live = await client.GetAsync("/health/live");
        using HttpResponseMessage ready = await client.GetAsync("/health/ready");
        live.EnsureSuccessStatusCode();
        ready.EnsureSuccessStatusCode();
    }

    private sealed record Course(
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("credits")] int Credits);

    private sealed record RuntimeShape(
        [property: JsonPropertyName("publishForm")] string PublishForm,
        [property: JsonPropertyName("framework")] string Framework,
        [property: JsonPropertyName("processArchitecture")] string ProcessArchitecture,
        [property: JsonPropertyName("isAotCompatible")] bool IsAotCompatible);

    private sealed record ValidationResult(
        [property: JsonPropertyName("valid")] bool Valid,
        [property: JsonPropertyName("reason")] string Reason);
}
