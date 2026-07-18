using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Patients.Infrastructure;

/// <summary>Implémentation du contrat public IPatientDirectory pour le module Patients.</summary>
internal sealed class PatientDirectory : IPatientDirectory
{
    private readonly IPatientsDbContext _context;

    public PatientDirectory(IPatientsDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(Guid patientId, CancellationToken cancellationToken = default) =>
        _context.Patients.AnyAsync(p => p.Id == patientId, cancellationToken);

    public async Task<bool> HasPreferentialRateAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);
        return patient?.Mutuality?.HasPreferentialRate ?? false;
    }
}
