using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.GetClinicalRecord;

public class GetClinicalRecordHandler : IQueryHandler<GetClinicalRecordQuery, Result<ClinicalRecordDto>>
{
    private readonly IPatientsDbContext _context;

    public GetClinicalRecordHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ClinicalRecordDto>> Handle(GetClinicalRecordQuery request, CancellationToken cancellationToken = default)
    {
        var profession = HealthProfession.FromCode(request.ProfessionCode);

        var record = await _context.ClinicalRecords
            .AsNoTracking()
            .Include(r => r.Entries)
            .Include(r => r.Prescriptions)
            .FirstOrDefaultAsync(
                r => r.PatientId == request.PatientId && r.Profession == profession,
                cancellationToken);

        if (record is null)
            return BusinessFailures.NotFound<ClinicalRecordDto>(
                $"Aucun dossier {profession.Code} pour le patient {request.PatientId}.");

        var dto = new ClinicalRecordDto(
            record.Id,
            record.PatientId,
            record.Profession.Code,
            record.OpenedOn,
            record.Entries
                .OrderBy(e => e.RecordedOn)
                .Select(e => new ClinicalEntryDto(
                    e.Id, e.Type.ToString(), e.Title, e.Content, e.AuthorPractitionerId, e.RecordedOn))
                .ToList(),
            record.Prescriptions
                .OrderBy(p => p.PrescribedOn)
                .Select(p => new PrescriptionDto(
                    p.Id, p.PrescriberName, p.PrescribedOn,
                    p.SessionsPrescribed, p.SessionsConsumed, p.RemainingSessions))
                .ToList());

        return Result.Success(dto);
    }
}
