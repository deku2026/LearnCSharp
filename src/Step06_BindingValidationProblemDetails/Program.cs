using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
        ctx.ProblemDetails.Extensions["environment"] = builder.Environment.EnvironmentName;
    };
});
builder.Services.AddSingleton<OrderStore>();
builder.Services.AddExceptionHandler<LabExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/", () => Results.Ok(new { lab = "Step06 Binding / Validation / ProblemDetails" }));

// Binding sources: route + query + header + body (+ manual validation for lab clarity)
app.MapPost("/api/orders/{studentNumber}", (
    string studentNumber,
    [FromQuery] string? coupon,
    [FromHeader(Name = "X-Request-Id")] string? requestId,
    CreateOrderBody body,
    OrderStore store) =>
{
    var errors = new Dictionary<string, string[]>();
    if (string.IsNullOrWhiteSpace(body.Sku) || body.Sku.Length < 3)
    {
        errors["sku"] = ["Sku is required (min 3)"];
    }

    if (body.Quantity is < 1 or > 99)
    {
        errors["quantity"] = ["Quantity must be 1..99"];
    }

    if (body.Sku.StartsWith("XX", StringComparison.OrdinalIgnoreCase) && body.Quantity > 1)
    {
        errors["sku"] = ["Limited SKU XX* allows quantity 1 only"];
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var order = store.Create(studentNumber, body.Sku, body.Quantity, coupon, requestId);
    return TypedResults.Created($"/api/orders/{order.Id}", order);
});

app.MapGet("/api/orders/{id:guid}", (Guid id, OrderStore store) =>
    store.Get(id) is { } o ? Results.Ok(o) : Results.NotFound());

// Force 404 ProblemDetails shape
app.MapGet("/api/missing", () => Results.NotFound());

// Unhandled → exception handler → ProblemDetails
app.MapGet("/api/crash", (HttpContext _) =>
{
    throw new InvalidOperationException("lab crash");
});

app.Run();

public partial class Program;

public sealed class CreateOrderBody : IValidatableObject
{
    [Required, MinLength(3)]
    public string Sku { get; set; } = "";

    [Range(1, 99)]
    public int Quantity { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Sku.StartsWith("XX", StringComparison.OrdinalIgnoreCase) && Quantity > 1)
        {
            yield return new ValidationResult(
                "Limited SKU XX* allows quantity 1 only",
                [nameof(Sku), nameof(Quantity)]);
        }
    }
}

public sealed record OrderDto(Guid Id, string StudentNumber, string Sku, int Quantity, string? Coupon, string? RequestId);

public sealed class OrderStore
{
    private readonly ConcurrentDictionary<Guid, OrderDto> _orders = new();

    public OrderDto Create(string studentNumber, string sku, int qty, string? coupon, string? requestId)
    {
        var o = new OrderDto(Guid.NewGuid(), studentNumber, sku, qty, coupon, requestId);
        _orders[o.Id] = o;
        return o;
    }

    public OrderDto? Get(Guid id) => _orders.GetValueOrDefault(id);
}

public sealed class LabExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await problemDetails.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Title = "Server error",
                Detail = exception.Message,
                Status = StatusCodes.Status500InternalServerError
            }
        });
        return true;
    }
}
