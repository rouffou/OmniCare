using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Practitioners.Domain.Entities;
using OmniCare.Modules.Practitioners.Domain.ValueObjects;
using OmniCare.Modules.Practitioners.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Practitioners.Features.RegisterCabinet;

public class RegisterCabinetHandler : ICommandHandler<RegisterCabinetCommand, Result<Guid>>
{
    private readonly IPractitionersDbContext _context;

    public RegisterCabinetHandler(IPractitionersDbContext context) => _context = context;

    public async Task<Result<Guid>> Handle(RegisterCabinetCommand request, CancellationToken cancellationToken = default)
    {
        BceNumber bceNumber;
        try
        {
            bceNumber = BceNumber.Create(request.BceNumber);
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        if (await _context.Cabinets.AnyAsync(c => c.BceNumber == bceNumber, cancellationToken))
            return BusinessFailures.Conflict<Guid>("Un cabinet avec ce numéro d'entreprise existe déjà.");

        var address = Address.Create(request.AddressLine, request.PostalCode, request.City);
        var cabinet = Cabinet.Register(request.Name, bceNumber, address);
        _context.Cabinets.Add(cabinet);

        return Result.Success(cabinet.Id);
    }
}
