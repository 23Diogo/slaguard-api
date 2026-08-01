using SlaGuard.Api.Domain;

namespace SlaGuard.Api.Services;

public sealed record CreateIncidentRequest(
    string Title,
    string System,
    string Owner,
    IncidentPriority Priority);

public sealed class IncidentService
{
    private readonly List<Incident> _incidents = [];
    private readonly object _lock = new();

    public Incident Create(CreateIncidentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("O título é obrigatório.");

        var incident = new Incident
        {
            Title = request.Title.Trim(),
            System = request.System.Trim(),
            Owner = request.Owner.Trim(),
            Priority = request.Priority,
            DueAt = DateTimeOffset.UtcNow.AddHours(GetSlaHours(request.Priority))
        };

        lock (_lock) _incidents.Add(incident);
        return incident;
    }

    public IReadOnlyCollection<Incident> Search(
        string? text,
        IncidentStatus? status,
        IncidentPriority? priority)
    {
        IEnumerable<Incident> query;
        lock (_lock) query = _incidents.ToArray();

        if (!string.IsNullOrWhiteSpace(text))
        {
            query = query.Where(item =>
                item.Title.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                item.System.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                item.Owner.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        if (status.HasValue) query = query.Where(item => item.Status == status);
        if (priority.HasValue) query = query.Where(item => item.Priority == priority);

        return query.OrderBy(item => item.DueAt).ToArray();
    }

    public Incident? ChangeStatus(Guid id, IncidentStatus status)
    {
        lock (_lock)
        {
            var incident = _incidents.FirstOrDefault(item => item.Id == id);
            incident?.ChangeStatus(status);
            return incident;
        }
    }

    public static int GetSlaHours(IncidentPriority priority) => priority switch
    {
        IncidentPriority.Critical => 2,
        IncidentPriority.High => 4,
        IncidentPriority.Medium => 8,
        _ => 24
    };
}
