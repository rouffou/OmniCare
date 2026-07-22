using OmniCare.Modules.Practitioners.Domain.Events;
using OmniCare.Modules.Practitioners.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Practitioners.Domain.Entities;

/// <summary>
/// Praticien rattaché à un cabinet — un praticien = un cabinet en Phase 1 (pas de
/// gestion multi-cabinets). Ticket #55.
/// </summary>
public sealed class Practitioner : AggregateRoot
{
    public PersonName Name { get; private set; }
    public HealthProfession Profession { get; private set; }
    public PractitionerInamiNumber InamiNumber { get; private set; }
    public Guid CabinetId { get; private set; }
    public PractitionerStatus Status { get; private set; }
    public DateTimeOffset RegisteredOn { get; private set; }

    private Practitioner(
        PersonName name,
        HealthProfession profession,
        PractitionerInamiNumber inamiNumber,
        Guid cabinetId)
    {
        Name = name;
        Profession = profession;
        InamiNumber = inamiNumber;
        CabinetId = cabinetId;
        Status = PractitionerStatus.Active;
        RegisteredOn = DateTimeOffset.UtcNow;
    }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private Practitioner()
    {
    }
#pragma warning restore CS8618

    public static Practitioner Register(
        PersonName name,
        HealthProfession profession,
        PractitionerInamiNumber inamiNumber,
        Guid cabinetId)
    {
        if (cabinetId == Guid.Empty)
            throw new DomainException("Un praticien doit être rattaché à un cabinet.");

        var practitioner = new Practitioner(name, profession, inamiNumber, cabinetId);
        practitioner.Raise(new PractitionerRegisteredEvent(practitioner.Id, cabinetId));
        return practitioner;
    }

    public void Deactivate()
    {
        Status = PractitionerStatus.Inactive;
    }

    public void Activate()
    {
        Status = PractitionerStatus.Active;
    }
}

public enum PractitionerStatus
{
    Active = 0,
    Inactive = 1,
}
