using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Patients.Features.RegisterPrescription;

/// <summary>
/// Enregistrement d'une prescription médicale (nombre de séances prescrites,
/// suivi de consommation — cahier des charges §4.1). Commande auditée.
/// </summary>
public record RegisterPrescriptionCommand(
    Guid PatientId,
    string ProfessionCode,
    string PrescriberName,
    DateOnly PrescribedOn,
    int SessionsPrescribed
) : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "ClinicalRecord.RegisterPrescription";
    public Guid? AuditTargetId => PatientId;
}
