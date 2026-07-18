using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Agenda.Domain.Entities;

/// <summary>
/// Salle ou équipement partagé du cabinet (cahier des charges §4.2, optionnel selon
/// cabinet). Référentiel de données transverse à toutes les professions — pas de
/// notion de profession ici, contrairement à <see cref="AppointmentType"/>.
/// </summary>
public sealed class Room : Entity
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private Room()
    {
    }
#pragma warning restore CS8618

    public static Room Define(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Le nom d'une salle est obligatoire.");

        return new Room
        {
            Name = name.Trim(),
            IsActive = true,
        };
    }

    public void Deactivate() => IsActive = false;
}
