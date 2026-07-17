using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Agenda.Features.DefineAppointmentType;

public class DefineAppointmentTypeHandler : ICommandHandler<DefineAppointmentTypeCommand, Result<Guid>>
{
    private readonly IAgendaDbContext _context;

    public DefineAppointmentTypeHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(DefineAppointmentTypeCommand request, CancellationToken cancellationToken = default)
    {
        var profession = HealthProfession.FromCode(request.ProfessionCode);
        var name = request.Name.Trim();

        var alreadyExists = await _context.AppointmentTypes
            .AnyAsync(t => t.Profession == profession && t.Name == name, cancellationToken);
        if (alreadyExists)
            return BusinessFailures.Conflict<Guid>(
                $"Le type de rendez-vous « {name} » existe déjà pour la profession {profession.Code}.");

        var type = AppointmentType.Define(
            name, profession, TimeSpan.FromMinutes(request.DefaultDurationMinutes));

        _context.AppointmentTypes.Add(type);

        return Result.Success(type.Id);
    }
}
