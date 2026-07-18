using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;

namespace OmniCare.Modules.Agenda.Features.GetWaitlist;

public class GetWaitlistHandler : IQueryHandler<GetWaitlistQuery, Result<IReadOnlyList<WaitlistEntryDto>>>
{
    private readonly IAgendaDbContext _context;

    public GetWaitlistHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<WaitlistEntryDto>>> Handle(
        GetWaitlistQuery request,
        CancellationToken cancellationToken = default)
    {
        var entries = await _context.WaitlistEntries
            .AsNoTracking()
            .Where(w => w.PractitionerId == request.PractitionerId && w.Status == WaitlistStatus.Waiting)
            .OrderBy(w => w.JoinedOn)
            .Select(w => new WaitlistEntryDto(
                w.Id, w.PatientId, w.AppointmentTypeId, w.RequestedFrom, w.RequestedTo, w.Notes, w.JoinedOn))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<WaitlistEntryDto>>(entries);
    }
}
