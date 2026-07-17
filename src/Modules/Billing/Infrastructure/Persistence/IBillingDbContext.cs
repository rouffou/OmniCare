using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Domain.Entities;

namespace OmniCare.Modules.Billing.Infrastructure.Persistence;

/// <summary>Abstraction du contexte de persistance consommée par les handlers du module.</summary>
public interface IBillingDbContext
{
    DbSet<Invoice> Invoices { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
