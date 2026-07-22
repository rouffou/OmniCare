using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Practitioners.Domain.Entities;
using OmniCare.Modules.Practitioners.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain.ValueObjects;
using OmniCare.SharedKernel.Infrastructure;

namespace OmniCare.Modules.Practitioners.Infrastructure.Persistence;

public sealed class PractitionersDbContext : ModuleDbContext, IPractitionersDbContext
{
    public PractitionersDbContext(DbContextOptions<PractitionersDbContext> options, IPublisher publisher)
        : base(options, publisher)
    {
    }

    public DbSet<Cabinet> Cabinets => Set<Cabinet>();
    public DbSet<Practitioner> Practitioners => Set<Practitioner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("practitioners");

        modelBuilder.Entity<Cabinet>(cabinet =>
        {
            cabinet.ToTable("Cabinets");
            cabinet.HasKey(c => c.Id);

            cabinet.Property(c => c.Name).IsRequired().HasMaxLength(200);

            cabinet.Property(c => c.BceNumber)
                .HasConversion(b => b.Value, v => BceNumber.Create(v))
                .HasMaxLength(10)
                .HasColumnName("BceNumber")
                .IsRequired();
            cabinet.HasIndex(c => c.BceNumber).IsUnique();

            cabinet.OwnsOne(c => c.Address, address =>
            {
                address.Property(a => a.Line).HasColumnName("AddressLine").HasMaxLength(200).IsRequired();
                address.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(10).IsRequired();
                address.Property(a => a.City).HasColumnName("City").HasMaxLength(100).IsRequired();
            });

            cabinet.Property(c => c.RegisteredOn).HasConversion<UtcTicksConverter>();
        });

        modelBuilder.Entity<Practitioner>(practitioner =>
        {
            practitioner.ToTable("Practitioners");
            practitioner.HasKey(p => p.Id);

            practitioner.OwnsOne(p => p.Name, name =>
            {
                name.Property(n => n.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
                name.Property(n => n.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired();
            });

            practitioner.Property(p => p.Profession)
                .HasConversion(pr => pr.Code, code => HealthProfession.FromCode(code))
                .HasMaxLength(20)
                .IsRequired();

            practitioner.Property(p => p.InamiNumber)
                .HasConversion(i => i.Value, v => PractitionerInamiNumber.Create(v))
                .HasMaxLength(11)
                .HasColumnName("InamiNumber")
                .IsRequired();
            practitioner.HasIndex(p => p.InamiNumber).IsUnique();

            practitioner.HasIndex(p => p.CabinetId);
            practitioner.Property(p => p.RegisteredOn).HasConversion<UtcTicksConverter>();
        });

        UseClientGeneratedIds(modelBuilder);
    }
}
