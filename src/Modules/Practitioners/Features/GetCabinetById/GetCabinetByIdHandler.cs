using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Practitioners.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Practitioners.Features.GetCabinetById;

public class GetCabinetByIdHandler : IQueryHandler<GetCabinetByIdQuery, Result<CabinetDto>>
{
    private readonly IPractitionersDbContext _context;

    public GetCabinetByIdHandler(IPractitionersDbContext context) => _context = context;

    public async Task<Result<CabinetDto>> Handle(GetCabinetByIdQuery request, CancellationToken cancellationToken = default)
    {
        var cabinet = await _context.Cabinets
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CabinetId, cancellationToken);

        if (cabinet is null)
            return BusinessFailures.NotFound<CabinetDto>($"Cabinet {request.CabinetId} introuvable.");

        return Result.Success(new CabinetDto(
            cabinet.Id,
            cabinet.Name,
            cabinet.BceNumber.Formatted,
            cabinet.Address.Line,
            cabinet.Address.PostalCode,
            cabinet.Address.City,
            cabinet.IsActive));
    }
}
