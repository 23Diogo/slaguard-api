using SlaGuard.Api.Domain;
using SlaGuard.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IncidentService>();

var app = builder.Build();
app.MapOpenApi();

var incidents = app.MapGroup("/api/incidents").WithTags("Incidents");

incidents.MapPost("/", (CreateIncidentRequest request, IncidentService service) =>
{
    try
    {
        var created = service.Create(request);
        return Results.Created($"/api/incidents/{created.Id}", created);
    }
    catch (ArgumentException error)
    {
        return Results.BadRequest(new { error = error.Message });
    }
});

incidents.MapGet("/", (
    string? text,
    IncidentStatus? status,
    IncidentPriority? priority,
    IncidentService service) =>
    Results.Ok(service.Search(text, status, priority)));

incidents.MapPatch("/{id:guid}/status", (
    Guid id,
    IncidentStatus status,
    IncidentService service) =>
{
    var updated = service.ChangeStatus(id, status);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
});

app.MapGet("/api/dashboard", (IncidentService service) =>
{
    var items = service.Search(null, null, null);
    var now = DateTimeOffset.UtcNow;
    return Results.Ok(new
    {
        total = items.Count,
        open = items.Count(item => item.Status != IncidentStatus.Resolved),
        critical = items.Count(item => item.Priority == IncidentPriority.Critical),
        breached = items.Count(item => item.IsBreached(now))
    });
});

app.Run();

public partial class Program;
