using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Patients.Features.DownloadClinicalDocument;

public record DownloadClinicalDocumentQuery(Guid DocumentId) : IQuery<Result<DownloadedDocument>>;

public record DownloadedDocument(byte[] Content, string FileName, string ContentType);
