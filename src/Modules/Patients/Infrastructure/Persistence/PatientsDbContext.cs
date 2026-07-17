using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.SharedKernel.Domain.ValueObjects;
using OmniCare.SharedKernel.Infrastructure;

namespace OmniCare.Modules.Patients.Infrastructure.Persistence;

public sealed class PatientsDbContext : ModuleDbContext, IPatientsDbContext
{
    public PatientsDbContext(DbContextOptions<PatientsDbContext> options, IPublisher publisher)
        : base(options, publisher)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<ClinicalRecord> ClinicalRecords => Set<ClinicalRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("patients");

        modelBuilder.Entity<Patient>(patient =>
        {
            patient.ToTable("Patients");
            patient.HasKey(p => p.Id);

            patient.OwnsOne(p => p.Name, name =>
            {
                name.Property(n => n.FirstName).HasMaxLength(100).HasColumnName("FirstName");
                name.Property(n => n.LastName).HasMaxLength(100).HasColumnName("LastName");
            });

            patient.Property(p => p.NationalRegistryNumber)
                .HasConversion(v => v!.Value, s => NationalRegistryNumber.Create(s))
                .HasMaxLength(11)
                .HasColumnName("Niss");
            patient.HasIndex(p => p.NationalRegistryNumber).IsUnique();

            patient.OwnsOne(p => p.Contact);
            patient.OwnsOne(p => p.Mutuality);
            patient.OwnsOne(p => p.Insurability, insurability =>
            {
                insurability.Property(i => i.State).HasColumnName("InsurabilityState");
                insurability.Property(i => i.LastCheckedOn).HasColumnName("InsurabilityCheckedOn");
            });

            patient.Property(p => p.TreatingPhysicianName).HasMaxLength(200);
            patient.Property(p => p.EmergencyContact).HasMaxLength(300);

            patient.HasMany(p => p.Consents)
                .WithOne()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Consent>(consent =>
        {
            consent.ToTable("Consents");
            consent.HasKey(c => c.Id);
        });

        modelBuilder.Entity<ClinicalRecord>(record =>
        {
            record.ToTable("ClinicalRecords");
            record.HasKey(r => r.Id);

            record.Property(r => r.Profession)
                .HasConversion(p => p.Code, code => HealthProfession.FromCode(code))
                .HasMaxLength(20);

            // Un seul dossier par patient et par profession de santé.
            record.HasIndex(r => new { r.PatientId, r.Profession }).IsUnique();

            record.HasMany(r => r.Entries)
                .WithOne()
                .HasForeignKey(e => e.ClinicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            record.HasMany(r => r.Prescriptions)
                .WithOne()
                .HasForeignKey(p => p.ClinicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClinicalEntry>(entry =>
        {
            entry.ToTable("ClinicalEntries");
            entry.HasKey(e => e.Id);
            entry.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Prescription>(prescription =>
        {
            prescription.ToTable("Prescriptions");
            prescription.HasKey(p => p.Id);
            prescription.Property(p => p.PrescriberName).HasMaxLength(200);
        });

        UseClientGeneratedIds(modelBuilder);
    }
}
