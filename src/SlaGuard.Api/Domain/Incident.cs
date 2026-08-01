namespace SlaGuard.Api.Domain;

public enum IncidentPriority { Low, Medium, High, Critical }
public enum IncidentStatus { Open, InProgress, Resolved }

public sealed class Incident
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; init; }
    public required string System { get; init; }
    public required string Owner { get; init; }
    public IncidentPriority Priority { get; init; }
    public IncidentStatus Status { get; private set; } = IncidentStatus.Open;
    public DateTimeOffset OpenedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset DueAt { get; init; }

    public bool IsBreached(DateTimeOffset now) =>
        Status != IncidentStatus.Resolved && DueAt < now;

    public void ChangeStatus(IncidentStatus status) => Status = status;
}
