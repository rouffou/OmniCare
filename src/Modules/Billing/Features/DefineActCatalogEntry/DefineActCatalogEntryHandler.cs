using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Domain.Entities;
using OmniCare.Modules.Billing.Domain.ValueObjects;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Billing.Features.DefineActCatalogEntry;

public class DefineActCatalogEntryHandler : ICommandHandler<DefineActCatalogEntryCommand, Result<Guid>>
{
    private readonly IBillingDbContext _context;

    public DefineActCatalogEntryHandler(IBillingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(DefineActCatalogEntryCommand request, CancellationToken cancellationToken = default)
    {
        var profession = HealthProfession.FromCode(request.ProfessionCode);
        var code = InamiCode.Create(request.CodeStr);

        var alreadyExists = await _context.ActCatalogEntries.AnyAsync(
            e => e.Profession == profession && e.Code == code, cancellationToken);
        if (alreadyExists)
            return BusinessFailures.Conflict<Guid>(
                $"Le code {code.Value} existe déjà au référentiel de la profession {profession.Code}.");

        var entry = ActCatalogEntry.Define(profession, code, request.Label, Amount.Create(request.DefaultTariff));
        _context.ActCatalogEntries.Add(entry);

        return Result.Success(entry.Id);
    }
}
