using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Practitioners.Features.GetPractitionerById;

public record GetPractitionerByIdQuery(Guid PractitionerId) : IQuery<Result<PractitionerDto>>;

public record PractitionerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string ProfessionCode,
    string InamiNumber,
    Guid CabinetId,
    string Status);
