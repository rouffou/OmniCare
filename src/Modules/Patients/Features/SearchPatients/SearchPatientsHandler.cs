using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Infrastructure.Persistence;

namespace OmniCare.Modules.Patients.Features.SearchPatients;

public class SearchPatientsHandler : IQueryHandler<SearchPatientsQuery, Result<SearchPatientsResult>>
{
    private readonly IPatientsDbContext _context;

    public SearchPatientsHandler(IPatientsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SearchPatientsResult>> Handle(SearchPatientsQuery request, CancellationToken cancellationToken = default)
    {
        var query = _context.Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(p =>
                EF.Functions.Like(p.Name.LastName, $"%{term}%") ||
                EF.Functions.Like(p.Name.FirstName, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<PatientStatus>(request.Status, ignoreCase: true, out var status))
        {
            query = query.Where(p => p.Status == status);
        }

        if (request.ReferentPractitionerId.HasValue)
            query = query.Where(p => p.ReferentPractitionerId == request.ReferentPractitionerId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name.LastName).ThenBy(p => p.Name.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PatientSummaryDto(
                p.Id,
                p.Name.FirstName,
                p.Name.LastName,
                p.BirthDate,
                p.Contact.Phone,
                p.Contact.Email,
                p.Status.ToString(),
                p.ReferentPractitionerId))
            .ToListAsync(cancellationToken);

        return Result.Success(new SearchPatientsResult(items, totalCount, request.Page, request.PageSize));
    }
}
