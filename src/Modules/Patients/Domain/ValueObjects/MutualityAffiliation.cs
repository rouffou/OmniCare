using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Domain.ValueObjects;

/// <summary>
/// Affiliation du patient à un organisme assureur (mutuelle).
/// <see cref="HasPreferentialRate"/> couvre le régime BIM/OMNIO (tiers payant, tickets modérateurs réduits).
/// </summary>
public sealed record MutualityAffiliation
{
    public string MutualityCode { get; private set; }
    public string? MemberNumber { get; private set; }
    public bool HasPreferentialRate { get; private set; }

    private MutualityAffiliation(string mutualityCode, string? memberNumber, bool hasPreferentialRate)
    {
        MutualityCode = mutualityCode;
        MemberNumber = memberNumber;
        HasPreferentialRate = hasPreferentialRate;
    }

    public static MutualityAffiliation Create(string mutualityCode, string? memberNumber = null, bool hasPreferentialRate = false)
    {
        if (string.IsNullOrWhiteSpace(mutualityCode))
            throw new DomainException("Le code de l'organisme assureur est obligatoire.");
        return new MutualityAffiliation(mutualityCode.Trim(),
            string.IsNullOrWhiteSpace(memberNumber) ? null : memberNumber.Trim(),
            hasPreferentialRate);
    }
}
