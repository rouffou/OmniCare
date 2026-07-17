namespace OmniCare.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Profession de santé du praticien. Détermine les champs de dossier, la nomenclature
/// d'actes et les règles métier applicables (cahier des charges §4.6). Modélisée en
/// référentiel extensible plutôt qu'en enum figée : l'ajout d'une profession ne doit
/// pas exiger de refonte du Domain.
/// </summary>
public sealed record HealthProfession
{
    public string Code { get; }
    public string DisplayName { get; }

    private HealthProfession(string code, string displayName)
    {
        Code = code;
        DisplayName = displayName;
    }

    public static readonly HealthProfession Physiotherapy = new("PHYSIO", "Kinésithérapie");
    public static readonly HealthProfession GeneralMedicine = new("GP", "Médecine générale");
    public static readonly HealthProfession Nursing = new("NURSE", "Soins infirmiers");
    public static readonly HealthProfession Dentistry = new("DENTIST", "Dentisterie");
    public static readonly HealthProfession Psychology = new("PSY", "Psychologie");

    public static readonly IReadOnlyList<HealthProfession> All =
        [Physiotherapy, GeneralMedicine, Nursing, Dentistry, Psychology];

    public static HealthProfession FromCode(string code)
    {
        var match = All.FirstOrDefault(p =>
            string.Equals(p.Code, code?.Trim(), StringComparison.OrdinalIgnoreCase));
        return match ?? throw new DomainException(
            $"Profession de santé inconnue : « {code} ». Codes valides : {string.Join(", ", All.Select(p => p.Code))}.");
    }

    public override string ToString() => Code;
}
