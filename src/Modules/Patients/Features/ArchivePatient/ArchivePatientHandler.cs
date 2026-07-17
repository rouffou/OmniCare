using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Patients.Features.ArchivePatient;

public class ArchivePatientHandler : ICommandHandler<ArchivePatientCommand, Result<Guid>>
{
    private readonly IPatientsDbContext _context;

    public ArchivePatientHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(ArchivePatientCommand request, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient is null)
            return BusinessFailures.NotFound<Guid>($"Patient {request.PatientId} introuvable.");

        // Idempotent : archiver une fiche déjà archivée est un no-op côté Domain.
        patient.Archive();
        return Result.Success(patient.Id);
    }
}
