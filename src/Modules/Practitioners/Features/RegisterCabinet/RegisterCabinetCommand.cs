using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Practitioners.Features.RegisterCabinet;

public record RegisterCabinetCommand(
    string Name,
    string BceNumber,
    string AddressLine,
    string PostalCode,
    string City
) : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Cabinet.Register";
    public Guid? AuditTargetId => null;
}
