using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Practitioners.Domain.Entities;
using OmniCare.Modules.Practitioners.Domain.ValueObjects;
using OmniCare.Modules.Practitioners.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Practitioners.Features.RegisterPractitioner;

public class RegisterPractitionerHandler : ICommandHandler<RegisterPractitionerCommand, Result<Guid>>
{
    private readonly IPractitionersDbContext _context;

    public RegisterPractitionerHandler(IPractitionersDbContext context) => _context = context;

    public async Task<Result<Guid>> Handle(RegisterPractitionerCommand request, CancellationToken cancellationToken = default)
    {
        HealthProfession profession;
        PractitionerInamiNumber inamiNumber;
        try
        {
            profession = HealthProfession.FromCode(request.ProfessionCode);
            inamiNumber = PractitionerInamiNumber.Create(request.InamiNumber);
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        var cabinetExists = await _context.Cabinets.AnyAsync(c => c.Id == request.CabinetId, cancellationToken);
        if (!cabinetExists)
            return BusinessFailures.NotFound<Guid>($"Cabinet {request.CabinetId} introuvable.");

        if (await _context.Practitioners.AnyAsync(p => p.InamiNumber == inamiNumber, cancellationToken))
            return BusinessFailures.Conflict<Guid>("Un praticien avec ce numéro INAMI existe déjà.");

        var practitioner = Practitioner.Register(
            PersonName.Create(request.FirstName, request.LastName),
            profession,
            inamiNumber,
            request.CabinetId);
        _context.Practitioners.Add(practitioner);

        return Result.Success(practitioner.Id);
    }
}
