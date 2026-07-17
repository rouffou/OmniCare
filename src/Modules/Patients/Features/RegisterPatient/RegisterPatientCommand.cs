using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Patients.Features.RegisterPatient;

/// <summary>Création d'une fiche patient (données administratives uniquement).</summary>
public record RegisterPatientCommand(
    string FirstName,
    string LastName,
    string? NationalRegistryNumber,
    DateOnly? BirthDate,
    string? Email,
    string? Phone,
    string? AddressLine,
    string? PostalCode,
    string? City,
    string? MutualityCode,
    string? MutualityMemberNumber,
    bool HasPreferentialRate,
    string? TreatingPhysicianName,
    string? EmergencyContact,
    Guid? ReferentPractitionerId
) : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Patient.Register";
    public Guid? AuditTargetId => null;
}
