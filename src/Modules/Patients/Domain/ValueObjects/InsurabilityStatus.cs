namespace OmniCare.Modules.Patients.Domain.ValueObjects;

/// <summary>
/// Résultat de la dernière vérification d'assurabilité (MyCareNet).
/// Toute facturation exigera un statut vérifié et à jour (module Billing, Phase 2).
/// </summary>
public sealed record InsurabilityStatus
{
    public InsurabilityState State { get; private set; }
    public DateOnly? LastCheckedOn { get; private set; }

    private InsurabilityStatus(InsurabilityState state, DateOnly? lastCheckedOn)
    {
        State = state;
        LastCheckedOn = lastCheckedOn;
    }

    public static readonly InsurabilityStatus Unknown = new(InsurabilityState.Unknown, null);

    public static InsurabilityStatus Checked(bool insured, DateOnly checkedOn) =>
        new(insured ? InsurabilityState.Insured : InsurabilityState.NotInsured, checkedOn);
}

public enum InsurabilityState
{
    Unknown = 0,
    Insured = 1,
    NotInsured = 2,
}
