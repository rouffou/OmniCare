using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.RecordClinicalEntry;

public class RecordClinicalEntryHandler : ICommandHandler<RecordClinicalEntryCommand, Result<Guid>>
{
    private readonly IPatientsDbContext _context;

    public RecordClinicalEntryHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(RecordClinicalEntryCommand request, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient is null)
            return BusinessFailures.NotFound<Guid>($"Patient {request.PatientId} introuvable.");
        if (patient.Status != PatientStatus.Active)
            return BusinessFailures.Rule<Guid>("Impossible d'écrire dans le dossier d'un patient archivé.");

        var profession = HealthProfession.FromCode(request.ProfessionCode);
        var entryType = Enum.Parse<ClinicalEntryType>(request.EntryType, ignoreCase: true);

        var record = await _context.ClinicalRecords
            .Include(r => r.Entries)
            .FirstOrDefaultAsync(
                r => r.PatientId == request.PatientId && r.Profession == profession,
                cancellationToken);

        if (record is null)
        {
            record = ClinicalRecord.Open(request.PatientId, profession);
            _context.ClinicalRecords.Add(record);
        }

        var entry = record.AddEntry(entryType, request.Content, request.AuthorPractitionerId, request.Title);

        return Result.Success(entry.Id);
    }
}
