using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Practitioners.Domain.Entities;

namespace OmniCare.Modules.Practitioners.Infrastructure.Persistence;

/// <summary>Abstraction du contexte de persistance consommée par les handlers du module.</summary>
public interface IPractitionersDbContext
{
    DbSet<Cabinet> Cabinets { get; }
    DbSet<Practitioner> Practitioners { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
