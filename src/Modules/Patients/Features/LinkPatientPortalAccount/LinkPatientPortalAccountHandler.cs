using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Features.LinkPatientPortalAccount;

public class LinkPatientPortalAccountHandler
    : ICommandHandler<LinkPatientPortalAccountCommand, Result<Guid>>
{
    private readonly IPatientsDbContext _context;

    public LinkPatientPortalAccountHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        LinkPatientPortalAccountCommand request, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient is null)
            return BusinessFailures.NotFound<Guid>($"Patient {request.PatientId} introuvable.");

        var alreadyLinkedToAnother = await _context.Patients
            .AnyAsync(
                p => p.Id != request.PatientId && p.PortalUserId == request.PortalUserId,
                cancellationToken);
        if (alreadyLinkedToAnother)
            return BusinessFailures.Conflict<Guid>(
                "Ce compte portail est déjà lié à une autre fiche patient.");

        try
        {
            patient.LinkPortalAccount(request.PortalUserId);
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        return Result.Success(patient.Id);
    }
}
