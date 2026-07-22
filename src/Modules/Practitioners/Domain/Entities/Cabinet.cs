using OmniCare.Modules.Practitioners.Domain.Events;
using OmniCare.Modules.Practitioners.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Practitioners.Domain.Entities;

/// <summary>
/// Cabinet exerçant l'activité de soins — identification légale requise sur les
/// documents de facturation (cahier des charges §4.3, déclenché par le besoin de
/// facture PDF conforme, cf. ticket #38).
/// </summary>
public sealed class Cabinet : AggregateRoot
{
    public string Name { get; private set; }
    public BceNumber BceNumber { get; private set; }
    public Address Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset RegisteredOn { get; private set; }

    private Cabinet(string name, BceNumber bceNumber, Address address)
    {
        Name = name;
        BceNumber = bceNumber;
        Address = address;
        IsActive = true;
        RegisteredOn = DateTimeOffset.UtcNow;
    }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private Cabinet()
    {
    }
#pragma warning restore CS8618

    public static Cabinet Register(string name, BceNumber bceNumber, Address address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Le nom du cabinet est obligatoire.");

        var cabinet = new Cabinet(name.Trim(), bceNumber, address);
        cabinet.Raise(new CabinetRegisteredEvent(cabinet.Id));
        return cabinet;
    }

    public void UpdateAddress(Address address)
    {
        EnsureActive();
        Address = address;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void EnsureActive()
    {
        if (!IsActive)
            throw new DomainException("Opération impossible : le cabinet est désactivé.");
    }
}
