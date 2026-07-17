namespace OmniCare.Modules.Billing.Infrastructure.Services;

/// <summary>
/// Abstraction des échanges MyCareNet consommée par les handlers du module
/// (vérification d'assurabilité avant facturation — cahier des charges §4.3).
/// </summary>
public interface IMyCareNetService
{
    Task<bool> VerifyAssurabilityAsync(Guid patientId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implémentation de développement : considère tout patient assuré. Le client réel
/// (réseau eHealth/MyCareNet) sera marqué IResilientRequest et passera par
/// Mediarq.Polly (retry/timeout/circuit breaker) — Phase 2, intégration réelle.
/// </summary>
public sealed class FakeMyCareNetService : IMyCareNetService
{
    public Task<bool> VerifyAssurabilityAsync(Guid patientId, CancellationToken cancellationToken = default) =>
        Task.FromResult(true);
}
