using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Features.DownloadClinicalDocument;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.Modules.Patients.Infrastructure.Storage;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Patients.Features.PortalDownloadMyClinicalDocument;

public class PortalDownloadClinicalDocumentHandler
    : IQueryHandler<PortalDownloadClinicalDocumentQuery, Result<DownloadedDocument>>
{
    private readonly IPatientsDbContext _context;
    private readonly IDocumentStorage _storage;

    public PortalDownloadClinicalDocumentHandler(IPatientsDbContext context, IDocumentStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<Result<DownloadedDocument>> Handle(
        PortalDownloadClinicalDocumentQuery request, CancellationToken cancellationToken = default)
    {
        var document = await _context.ClinicalDocuments
            .AsNoTracking()
            .Join(
                _context.ClinicalRecords.AsNoTracking(),
                d => d.ClinicalRecordId,
                r => r.Id,
                (d, r) => new { Document = d, r.PatientId })
            .Where(x => x.Document.Id == request.DocumentId)
            .FirstOrDefaultAsync(cancellationToken);

        // Même message pour "inexistant" et "appartient à un autre patient" : ne jamais
        // révéler à un patient qu'un document identifié par cet id existe ailleurs.
        if (document is null || document.PatientId != request.PatientId)
            return BusinessFailures.NotFound<DownloadedDocument>($"Document {request.DocumentId} introuvable.");

        var content = await _storage.ReadAsync(document.Document.StorageKey, cancellationToken);
        return Result.Success(new DownloadedDocument(content, document.Document.FileName, document.Document.ContentType));
    }
}
