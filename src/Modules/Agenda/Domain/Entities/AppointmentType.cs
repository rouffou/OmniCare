using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Agenda.Domain.Entities;

/// <summary>
/// Type d'acte planifiable, configurable par profession de santé (durée par défaut
/// paramétrable : séance de 30 min chez un kiné, consultation de 15 min chez un
/// généraliste — cahier des charges §4.2 et §4.6). Référentiel de données, pas d'enum.
/// </summary>
public sealed class AppointmentType : Entity
{
    public string Name { get; private set; }
    public HealthProfession Profession { get; private set; }
    public TimeSpan DefaultDuration { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private AppointmentType()
    {
    }
#pragma warning restore CS8618

    public static AppointmentType Define(string name, HealthProfession profession, TimeSpan defaultDuration)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Le nom d'un type de rendez-vous est obligatoire.");
        if (defaultDuration <= TimeSpan.Zero || defaultDuration > TimeSpan.FromHours(8))
            throw new DomainException("La durée par défaut d'un type de rendez-vous doit être comprise entre 1 minute et 8 heures.");

        return new AppointmentType
        {
            Name = name.Trim(),
            Profession = profession,
            DefaultDuration = defaultDuration,
            IsActive = true,
        };
    }

    public void Deactivate() => IsActive = false;
}
