using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Practitioners.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Practitioners.Features.GetPractitionerById;

public class GetPractitionerByIdHandler : IQueryHandler<GetPractitionerByIdQuery, Result<PractitionerDto>>
{
    private readonly IPractitionersDbContext _context;

    public GetPractitionerByIdHandler(IPractitionersDbContext context) => _context = context;

    public async Task<Result<PractitionerDto>> Handle(GetPractitionerByIdQuery request, CancellationToken cancellationToken = default)
    {
        var practitioner = await _context.Practitioners
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PractitionerId, cancellationToken);

        if (practitioner is null)
            return BusinessFailures.NotFound<PractitionerDto>($"Praticien {request.PractitionerId} introuvable.");

        return Result.Success(new PractitionerDto(
            practitioner.Id,
            practitioner.Name.FirstName,
            practitioner.Name.LastName,
            practitioner.Profession.Code,
            practitioner.InamiNumber.Value,
            practitioner.CabinetId,
            practitioner.Status.ToString()));
    }
}
