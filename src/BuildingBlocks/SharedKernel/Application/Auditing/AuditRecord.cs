namespace OmniCare.SharedKernel.Application.Auditing;

/// <summary>Entrée immuable du journal d'audit.</summary>
public sealed record AuditRecord
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Action { get; init; }
    public required string RequestType { get; init; }
    public Guid? TargetId { get; init; }
    public required string UserId { get; init; }
    public required DateTimeOffset OccurredOn { get; init; }
    public required bool Succeeded { get; init; }
    public string? FailureReason { get; init; }
}

/// <summary>Persistance du journal d'audit (implémentée côté Api/Infrastructure).</summary>
public interface IAuditTrailStore
{
    Task AppendAsync(AuditRecord record, CancellationToken cancellationToken = default);
}
