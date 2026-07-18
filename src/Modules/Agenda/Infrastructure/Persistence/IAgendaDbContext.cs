using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;

namespace OmniCare.Modules.Agenda.Infrastructure.Persistence;

/// <summary>Abstraction du contexte de persistance consommée par les handlers du module.</summary>
public interface IAgendaDbContext
{
    DbSet<Appointment> Appointments { get; }
    DbSet<AppointmentType> AppointmentTypes { get; }
    DbSet<WaitlistEntry> WaitlistEntries { get; }
    DbSet<Room> Rooms { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
