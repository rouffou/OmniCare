using Microsoft.EntityFrameworkCore;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Api.Infrastructure.Auditing;

/// <summary>
/// Base de logs d'audit, volontairement séparée des contextes métier :
/// append-only, aucune écriture métier ne peut altérer le journal.
/// </summary>
public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditRecord>(record =>
        {
            record.ToTable("AuditTrail");
            record.HasKey(r => r.Id);
            record.Property(r => r.Action).HasMaxLength(200);
            record.Property(r => r.RequestType).HasMaxLength(200);
            record.Property(r => r.UserId).HasMaxLength(100);
            record.Property(r => r.FailureReason).HasMaxLength(2000);
            // Ticks UTC : permet les requêtes par plage de dates sous SQLite.
            record.Property(r => r.OccurredOn)
                .HasConversion<OmniCare.SharedKernel.Infrastructure.UtcTicksConverter>();
            record.HasIndex(r => r.TargetId);
            record.HasIndex(r => r.OccurredOn);
        });
    }
}

public sealed class EfCoreAuditTrailStore : IAuditTrailStore
{
    private readonly AuditDbContext _context;

    public EfCoreAuditTrailStore(AuditDbContext context)
    {
        _context = context;
    }

    public async Task AppendAsync(AuditRecord record, CancellationToken cancellationToken = default)
    {
        _context.AuditRecords.Add(record);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
