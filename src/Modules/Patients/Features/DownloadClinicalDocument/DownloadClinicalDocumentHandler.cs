using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.Modules.Patients.Infrastructure.Storage;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Patients.Features.DownloadClinicalDocument;

public class DownloadClinicalDocumentHandler : IQueryHandler<DownloadClinicalDocumentQuery, Result<DownloadedDocument>>
{
    private readonly IPatientsDbContext _context;
    private readonly IDocumentStorage _storage;

    public DownloadClinicalDocumentHandler(IPatientsDbContext context, IDocumentStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<Result<DownloadedDocument>> Handle(
        DownloadClinicalDocumentQuery request, CancellationToken cancellationToken = default)
    {
        var document = await _context.ClinicalDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);
        if (document is null)
            return BusinessFailures.NotFound<DownloadedDocument>($"Document {request.DocumentId} introuvable.");

        var content = await _storage.ReadAsync(document.StorageKey, cancellationToken);
        return Result.Success(new DownloadedDocument(content, document.FileName, document.ContentType));
    }
}
