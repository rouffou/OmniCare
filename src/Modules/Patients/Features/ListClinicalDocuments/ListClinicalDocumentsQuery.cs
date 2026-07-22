using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Patients.Features.ListClinicalDocuments;

public record ListClinicalDocumentsQuery(Guid PatientId, string ProfessionCode)
    : IQuery<Result<IReadOnlyList<ClinicalDocumentDto>>>;

public record ClinicalDocumentDto(
    Guid Id,
    string Type,
    string FileName,
    string ContentType,
    long SizeBytes,
    Guid UploadedByPractitionerId,
    DateTimeOffset UploadedOn);
