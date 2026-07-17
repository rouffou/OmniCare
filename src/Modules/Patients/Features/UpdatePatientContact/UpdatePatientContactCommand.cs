using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Patients.Features.UpdatePatientContact;

/// <summary>
/// Mise à jour des coordonnées administratives du patient (vue secrétariat).
/// Donnée administrative : pas d'audit trail médical requis.
/// </summary>
public record UpdatePatientContactCommand(
    Guid PatientId,
    string? Email,
    string? Phone,
    string? AddressLine,
    string? PostalCode,
    string? City
) : ICommand<Result<Guid>>, ITransactionalRequest;
