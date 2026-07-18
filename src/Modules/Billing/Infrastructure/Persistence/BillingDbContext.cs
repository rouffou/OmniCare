using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Domain.Entities;
using OmniCare.Modules.Billing.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain.ValueObjects;
using OmniCare.SharedKernel.Infrastructure;

namespace OmniCare.Modules.Billing.Infrastructure.Persistence;

public sealed class BillingDbContext : ModuleDbContext, IBillingDbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options, IPublisher publisher)
        : base(options, publisher)
    {
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<ActCatalogEntry> ActCatalogEntries => Set<ActCatalogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("billing");

        modelBuilder.Entity<Invoice>(invoice =>
        {
            invoice.ToTable("Invoices");
            invoice.HasKey(i => i.Id);

            invoice.Property(i => i.Code)
                .HasConversion(c => c.Value, s => InamiCode.Create(s))
                .HasMaxLength(6)
                .HasColumnName("InamiCode");

            invoice.Property(i => i.Total)
                .HasConversion(a => a.Value, v => Amount.Create(v))
                .HasColumnType("decimal(10,2)");

            invoice.Property(i => i.PatientShare)
                .HasConversion(a => a.Value, v => Amount.Create(v))
                .HasColumnType("decimal(10,2)");

            invoice.Property(i => i.MutualityShare)
                .HasConversion(a => a.Value, v => Amount.Create(v))
                .HasColumnType("decimal(10,2)");

            // Ticks UTC : colonne triée/filtrée par plage de dates (SQLite ne traduit
            // pas les comparaisons sur DateTimeOffset).
            invoice.Property(i => i.IssuedOn).HasConversion<UtcTicksConverter>();

            invoice.Property(i => i.PaymentMethod).HasMaxLength(50);
            invoice.Property(i => i.CancellationReason).HasMaxLength(500);

            invoice.HasIndex(i => i.PatientId);
            invoice.HasIndex(i => new { i.PractitionerId, i.IssuedOn });
            invoice.HasIndex(i => i.Status);
        });

        modelBuilder.Entity<ActCatalogEntry>(entry =>
        {
            entry.ToTable("ActCatalogEntries");
            entry.HasKey(e => e.Id);

            entry.Property(e => e.Profession)
                .HasConversion(p => p.Code, code => HealthProfession.FromCode(code))
                .HasMaxLength(20);

            entry.Property(e => e.Code)
                .HasConversion(c => c.Value, s => InamiCode.Create(s))
                .HasMaxLength(6)
                .HasColumnName("InamiCode");

            entry.Property(e => e.Label).HasMaxLength(200);

            entry.Property(e => e.DefaultTariff)
                .HasConversion(a => a.Value, v => Amount.Create(v))
                .HasColumnType("decimal(10,2)");

            entry.HasIndex(e => new { e.Profession, e.Code }).IsUnique();
        });

        UseClientGeneratedIds(modelBuilder);
    }
}
