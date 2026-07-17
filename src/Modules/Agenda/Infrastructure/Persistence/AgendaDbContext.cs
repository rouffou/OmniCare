using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.SharedKernel.Domain.ValueObjects;
using OmniCare.SharedKernel.Infrastructure;

namespace OmniCare.Modules.Agenda.Infrastructure.Persistence;

public sealed class AgendaDbContext : ModuleDbContext, IAgendaDbContext
{
    public AgendaDbContext(DbContextOptions<AgendaDbContext> options, IPublisher publisher)
        : base(options, publisher)
    {
    }

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentType> AppointmentTypes => Set<AppointmentType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("agenda");

        modelBuilder.Entity<Appointment>(appointment =>
        {
            appointment.ToTable("Appointments");
            appointment.HasKey(a => a.Id);

            appointment.OwnsOne(a => a.Slot, slot =>
            {
                // Ticks UTC : SQLite ne traduit pas les comparaisons sur DateTimeOffset,
                // or ces colonnes portent les prédicats de chevauchement et le tri.
                slot.Property(s => s.Start).HasConversion<UtcTicksConverter>().HasColumnName("SlotStart");
                slot.Property(s => s.End).HasConversion<UtcTicksConverter>().HasColumnName("SlotEnd");
                slot.HasIndex(s => s.Start);
            });

            appointment.Property(a => a.Notes).HasMaxLength(1000);
            appointment.Property(a => a.CancellationReason).HasMaxLength(500);
            appointment.HasIndex(a => a.PractitionerId);
            appointment.HasIndex(a => a.PatientId);

            appointment.HasOne<AppointmentType>()
                .WithMany()
                .HasForeignKey(a => a.AppointmentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppointmentType>(type =>
        {
            type.ToTable("AppointmentTypes");
            type.HasKey(t => t.Id);
            type.Property(t => t.Name).HasMaxLength(200);
            type.Property(t => t.Profession)
                .HasConversion(p => p.Code, code => HealthProfession.FromCode(code))
                .HasMaxLength(20);
            type.HasIndex(t => new { t.Profession, t.Name }).IsUnique();
        });

        UseClientGeneratedIds(modelBuilder);
    }
}
