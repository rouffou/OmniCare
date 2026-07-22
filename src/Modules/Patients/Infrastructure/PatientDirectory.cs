using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
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

    public async Task<PatientIdentity?> GetIdentityAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .Include(p => p.Consents)
            .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);
        if (patient is null)
            return null;

        var hasElectronicCommunicationConsent = patient.Consents
            .Any(c => c.Type == ConsentType.ElectronicCommunication && c.IsActive);

        return new PatientIdentity(
            patient.Id,
            patient.Name.FullName,
            patient.Contact.AddressLine,
            patient.Contact.PostalCode,
            patient.Contact.City,
            patient.Contact.Email,
            patient.Contact.Phone,
            hasElectronicCommunicationConsent);
    }

    public async Task<Guid?> ResolvePatientIdByPortalUserIdAsync(
        string portalUserId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PortalUserId == portalUserId, cancellationToken);
        return patient?.Id;
    }
}
