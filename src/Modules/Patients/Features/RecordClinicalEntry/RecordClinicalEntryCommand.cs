using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Patients.Features.RecordClinicalEntry;

/// <summary>
/// Ajout d'une entrée au dossier médical (anamnèse, bilan clinique, plan de traitement,
/// compte-rendu de séance). Commande auditée : écriture dans un dossier médical.
/// Le dossier est ouvert automatiquement à la première entrée pour la profession donnée.
/// </summary>
public record RecordClinicalEntryCommand(
    Guid PatientId,
    string ProfessionCode,
    string EntryType,
    string Content,
    Guid AuthorPractitionerId,
    string? Title
) : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "ClinicalRecord.AddEntry";
    public Guid? AuditTargetId => PatientId;
}
