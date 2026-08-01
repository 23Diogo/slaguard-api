using SlaGuard.Api.Domain;
using SlaGuard.Api.Services;

namespace SlaGuard.Tests;

public sealed class IncidentServiceTests
{
    [Fact]
    public void Create_ShouldApplyCriticalSla()
    {
        var service = new IncidentService();
        var before = DateTimeOffset.UtcNow;

        var incident = service.Create(new(
            "Falha na integração", "ERP", "Diogo", IncidentPriority.Critical));

        Assert.InRange(incident.DueAt, before.AddHours(2), before.AddHours(2).AddSeconds(2));
        Assert.Equal(IncidentStatus.Open, incident.Status);
    }

    [Fact]
    public void Search_ShouldFilterByPriority()
    {
        var service = new IncidentService();
        service.Create(new("Erro crítico", "ERP", "Ana", IncidentPriority.Critical));
        service.Create(new("Ajuste visual", "Portal", "Carlos", IncidentPriority.Low));

        var result = service.Search(null, null, IncidentPriority.Critical);

        Assert.Single(result);
        Assert.Equal("Erro crítico", result.Single().Title);
    }
}
