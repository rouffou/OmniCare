using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Practitioners.Features.GetCabinetById;

public record GetCabinetByIdQuery(Guid CabinetId) : IQuery<Result<CabinetDto>>;

public record CabinetDto(
    Guid Id,
    string Name,
    string BceNumber,
    string AddressLine,
    string PostalCode,
    string City,
    bool IsActive);
