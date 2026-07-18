using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.DefineRoom;

public class DefineRoomHandler : ICommandHandler<DefineRoomCommand, Result<Guid>>
{
    private readonly IAgendaDbContext _context;

    public DefineRoomHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(DefineRoomCommand request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var alreadyExists = await _context.Rooms.AnyAsync(r => r.Name == name, cancellationToken);
        if (alreadyExists)
            return BusinessFailures.Conflict<Guid>($"La salle « {name} » existe déjà.");

        var room = Room.Define(name);
        _context.Rooms.Add(room);

        return Result.Success(room.Id);
    }
}
