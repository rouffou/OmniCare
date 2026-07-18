using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Agenda.Features.FulfillWaitlistEntry;

public class FulfillWaitlistEntryHandler : ICommandHandler<FulfillWaitlistEntryCommand, Result<Guid>>
{
    private readonly IAgendaDbContext _context;

    public FulfillWaitlistEntryHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(FulfillWaitlistEntryCommand request, CancellationToken cancellationToken = default)
    {
        var entry = await _context.WaitlistEntries
            .FirstOrDefaultAsync(w => w.Id == request.WaitlistEntryId, cancellationToken);
        if (entry is null)
            return BusinessFailures.NotFound<Guid>($"Inscription en liste d'attente {request.WaitlistEntryId} introuvable.");

        try
        {
            entry.MarkFulfilled();
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        return Result.Success(entry.Id);
    }
}
