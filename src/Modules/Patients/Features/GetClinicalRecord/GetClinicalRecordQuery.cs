using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Patients.Features.GetClinicalRecord;

/// <summary>
/// Consultation du dossier médical d'un patient pour une profession donnée.
/// Lecture auditée : la traçabilité des accès au dossier (qui a consulté quoi, quand)
/// est une exigence explicite du cahier des charges §4.1.
/// </summary>
public record GetClinicalRecordQuery(Guid PatientId, string ProfessionCode)
    : IQuery<Result<ClinicalRecordDto>>, IAuditableRequest
{
    public string AuditAction => "ClinicalRecord.View";
    public Guid? AuditTargetId => PatientId;
}

public record ClinicalRecordDto(
    Guid Id,
    Guid PatientId,
    string ProfessionCode,
    DateTimeOffset OpenedOn,
    IReadOnlyList<ClinicalEntryDto> Entries,
    IReadOnlyList<PrescriptionDto> Prescriptions);

public record ClinicalEntryDto(
    Guid Id,
    string Type,
    string? Title,
    string Content,
    Guid AuthorPractitionerId,
    DateTimeOffset RecordedOn);

public record PrescriptionDto(
    Guid Id,
    string PrescriberName,
    DateOnly PrescribedOn,
    int SessionsPrescribed,
    int SessionsConsumed,
    int RemainingSessions);
