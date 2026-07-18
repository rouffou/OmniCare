namespace OmniCare.Modules.Billing.Infrastructure.Services;

/// <summary>
/// Abstraction de la télétransmission des attestations de soins (eAttest) vers les
/// organismes assureurs via eHealth/MyCareNet (cahier des charges §4.3).
/// </summary>
public interface IEHealthTransmissionService
{
    Task<EAttestTransmissionResult> SubmitEAttestAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}

public sealed record EAttestTransmissionResult(bool Accepted, string? RejectionReason);

/// <summary>
/// Implémentation de développement : accepte systématiquement la télétransmission.
/// Le client réel (réseau eHealth/MyCareNet) sera marqué IResilientRequest et passera
/// par Mediarq.Polly (retry/timeout/circuit breaker) — intégration réelle hors périmètre
/// tant que l'accès eHealth n'est pas disponible.
/// </summary>
public sealed class FakeEHealthTransmissionService : IEHealthTransmissionService
{
    public Task<EAttestTransmissionResult> SubmitEAttestAsync(Guid invoiceId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new EAttestTransmissionResult(Accepted: true, RejectionReason: null));
}
