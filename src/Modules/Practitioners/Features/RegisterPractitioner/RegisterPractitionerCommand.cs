using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Practitioners.Features.RegisterPractitioner;

public record RegisterPractitionerCommand(
    string FirstName,
    string LastName,
    string ProfessionCode,
    string InamiNumber,
    Guid CabinetId
) : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Practitioner.Register";
    public Guid? AuditTargetId => CabinetId;
}
