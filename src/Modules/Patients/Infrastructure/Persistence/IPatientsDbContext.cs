using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;

namespace OmniCare.Modules.Patients.Infrastructure.Persistence;

/// <summary>Abstraction du contexte de persistance consommée par les handlers du module.</summary>
public interface IPatientsDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<ClinicalRecord> ClinicalRecords { get; }

    /// <summary>Exposé pour la lecture directe (téléchargement par id de document, sans
    /// connaître le dossier parent) — les écritures passent toujours par l'agrégat
    /// <see cref="ClinicalRecord"/> (<c>AddDocument</c>), jamais par ce DbSet.</summary>
    DbSet<ClinicalDocument> ClinicalDocuments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
