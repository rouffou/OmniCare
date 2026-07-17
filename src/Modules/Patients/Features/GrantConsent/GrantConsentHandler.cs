using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Features.GrantConsent;

public class GrantConsentHandler : ICommandHandler<GrantConsentCommand, Result<Guid>>
{
    private readonly IPatientsDbContext _context;

    public GrantConsentHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(GrantConsentCommand request, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .Include(p => p.Consents)
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient is null)
            return BusinessFailures.NotFound<Guid>($"Patient {request.PatientId} introuvable.");

        var consentType = Enum.Parse<ConsentType>(request.ConsentType, ignoreCase: true);
        try
        {
            // Idempotent si un consentement actif du même type existe déjà.
            patient.GrantConsent(consentType);
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        return Result.Success(patient.Id);
    }
}
