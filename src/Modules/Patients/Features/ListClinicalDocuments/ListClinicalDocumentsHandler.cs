using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.ListClinicalDocuments;

public class ListClinicalDocumentsHandler
    : IQueryHandler<ListClinicalDocumentsQuery, Result<IReadOnlyList<ClinicalDocumentDto>>>
{
    private readonly IPatientsDbContext _context;

    public ListClinicalDocumentsHandler(IPatientsDbContext context) => _context = context;

    public async Task<Result<IReadOnlyList<ClinicalDocumentDto>>> Handle(
        ListClinicalDocumentsQuery request, CancellationToken cancellationToken = default)
    {
        var profession = HealthProfession.FromCode(request.ProfessionCode);

        var record = await _context.ClinicalRecords
            .Include(r => r.Documents)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.PatientId == request.PatientId && r.Profession == profession,
                cancellationToken);

        // Pas encore de dossier clinique pour cette profession : liste vide, pas une erreur.
        if (record is null)
            return Result.Success<IReadOnlyList<ClinicalDocumentDto>>([]);

        var documents = record.Documents
            .OrderByDescending(d => d.UploadedOn)
            .Select(d => new ClinicalDocumentDto(
                d.Id, d.Type.ToString(), d.FileName, d.ContentType, d.SizeBytes,
                d.UploadedByPractitionerId, d.UploadedOn))
            .ToList();

        return Result.Success<IReadOnlyList<ClinicalDocumentDto>>(documents);
    }
}
