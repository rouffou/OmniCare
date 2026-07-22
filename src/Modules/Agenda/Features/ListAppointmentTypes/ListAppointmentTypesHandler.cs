using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Agenda.Features.ListAppointmentTypes;

public class ListAppointmentTypesHandler
    : IQueryHandler<ListAppointmentTypesQuery, Result<IReadOnlyList<AppointmentTypeDto>>>
{
    private readonly IAgendaDbContext _context;

    public ListAppointmentTypesHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<AppointmentTypeDto>>> Handle(
        ListAppointmentTypesQuery request, CancellationToken cancellationToken = default)
    {
        // Filtre sur la propriété convertie entière (traduisible en SQL) plutôt que sur
        // .Code, qui n'existe qu'après matérialisation de l'objet HealthProfession.
        var profession = request.ProfessionCode is null ? null : HealthProfession.FromCode(request.ProfessionCode);

        var entities = await _context.AppointmentTypes
            .AsNoTracking()
            .Where(t => t.IsActive && (profession == null || t.Profession == profession))
            .ToListAsync(cancellationToken);

        var items = entities
            .Select(t => new AppointmentTypeDto(t.Id, t.Name, t.Profession.Code, (int)t.DefaultDuration.TotalMinutes))
            .ToList();

        return Result.Success<IReadOnlyList<AppointmentTypeDto>>(items);
    }
}
