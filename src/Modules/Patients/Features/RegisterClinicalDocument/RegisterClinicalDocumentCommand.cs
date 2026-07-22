using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Patients.Features.RegisterClinicalDocument;

/// <summary>
/// Ajout d'un document joint au dossier médical (prescription scannée, imagerie… —
/// cahier des charges §4.1, ticket #31). Commande auditée : toute écriture dans le
/// dossier clinique est tracée.
/// </summary>
public record RegisterClinicalDocumentCommand(
    Guid PatientId,
    string ProfessionCode,
    string DocumentType,
    string FileName,
    string ContentType,
    string ContentBase64,
    Guid UploadedByPractitionerId
) : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "ClinicalRecord.AddDocument";
    public Guid? AuditTargetId => PatientId;
}
