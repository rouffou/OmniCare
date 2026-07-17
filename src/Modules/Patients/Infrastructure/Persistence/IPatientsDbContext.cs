using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;

namespace OmniCare.Modules.Patients.Infrastructure.Persistence;

/// <summary>Abstraction du contexte de persistance consommée par les handlers du module.</summary>
public interface IPatientsDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<ClinicalRecord> ClinicalRecords { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
