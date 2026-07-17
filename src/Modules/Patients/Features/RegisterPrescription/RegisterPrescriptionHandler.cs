using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.RegisterPrescription;

public class RegisterPrescriptionHandler : ICommandHandler<RegisterPrescriptionCommand, Result<Guid>>
{
    private readonly IPatientsDbContext _context;

    public RegisterPrescriptionHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(RegisterPrescriptionCommand request, CancellationToken cancellationToken = default)
    {
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == request.PatientId, cancellationToken);
        if (!patientExists)
            return BusinessFailures.NotFound<Guid>($"Patient {request.PatientId} introuvable.");

        var profession = HealthProfession.FromCode(request.ProfessionCode);

        var record = await _context.ClinicalRecords
            .Include(r => r.Prescriptions)
            .FirstOrDefaultAsync(
                r => r.PatientId == request.PatientId && r.Profession == profession,
                cancellationToken);

        if (record is null)
        {
            record = ClinicalRecord.Open(request.PatientId, profession);
            _context.ClinicalRecords.Add(record);
        }

        var prescription = record.RegisterPrescription(
            request.PrescriberName, request.PrescribedOn, request.SessionsPrescribed);

        return Result.Success(prescription.Id);
    }
}
