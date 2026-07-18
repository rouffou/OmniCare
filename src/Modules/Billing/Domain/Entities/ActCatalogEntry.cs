using OmniCare.Modules.Billing.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Billing.Domain.Entities;

/// <summary>
/// Entrée du référentiel d'actes facturables, par profession de santé (cahier des
/// charges §4.6 : la validité d'un code INAMI pour une profession donnée n'est pas
/// codée en dur, elle est pilotée par ce référentiel configurable). Chaque profession
/// dispose de ses propres codes ; un même code peut avoir un sens différent d'une
/// profession à l'autre — la clé métier est donc (Profession, Code).
/// </summary>
public sealed class ActCatalogEntry : Entity
{
    public HealthProfession Profession { get; private set; }
    public InamiCode Code { get; private set; }
    public string Label { get; private set; }
    public Amount DefaultTariff { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private ActCatalogEntry()
    {
    }
#pragma warning restore CS8618

    public static ActCatalogEntry Define(
        HealthProfession profession, InamiCode code, string label, Amount defaultTariff)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainException("Le libellé d'un acte est obligatoire.");
        if (defaultTariff.Value <= 0)
            throw new DomainException("Le tarif par défaut d'un acte doit être strictement positif.");

        return new ActCatalogEntry
        {
            Profession = profession,
            Code = code,
            Label = label.Trim(),
            DefaultTariff = defaultTariff,
            IsActive = true,
        };
    }

    public void Deactivate() => IsActive = false;
}
