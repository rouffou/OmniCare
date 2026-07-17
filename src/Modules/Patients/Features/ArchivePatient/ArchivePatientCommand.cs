using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Patients.Features.ArchivePatient;

/// <summary>
/// Archivage d'une fiche patient : plus aucune modification possible, mais le dossier
/// reste consultable (durées légales de conservation — cahier des charges §5.2 ;
/// la purge automatisée fera l'objet d'une politique dédiée).
/// </summary>
public record ArchivePatientCommand(Guid PatientId) : ICommand<Result<Guid>>, ITransactionalRequest;
