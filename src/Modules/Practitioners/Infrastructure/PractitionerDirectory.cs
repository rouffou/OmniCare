using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Practitioners.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Practitioners.Infrastructure;

internal sealed class PractitionerDirectory : IPractitionerDirectory
{
    private readonly IPractitionersDbContext _context;

    public PractitionerDirectory(IPractitionersDbContext context) => _context = context;

    public Task<bool> ExistsAsync(Guid practitionerId, CancellationToken cancellationToken = default) =>
        _context.Practitioners.AnyAsync(p => p.Id == practitionerId, cancellationToken);

    public async Task<PractitionerIdentity?> GetIdentityAsync(
        Guid practitionerId, CancellationToken cancellationToken = default)
    {
        var practitioner = await _context.Practitioners
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == practitionerId, cancellationToken);
        if (practitioner is null)
            return null;

        var cabinet = await _context.Cabinets
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == practitioner.CabinetId, cancellationToken);
        if (cabinet is null)
            return null;

        return new PractitionerIdentity(
            practitioner.Id,
            practitioner.Name.FullName,
            practitioner.Profession.Code,
            practitioner.InamiNumber.Value,
            cabinet.Id,
            cabinet.Name,
            cabinet.BceNumber.Formatted,
            cabinet.Address.Line,
            cabinet.Address.PostalCode,
            cabinet.Address.City);
    }
}
