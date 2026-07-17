using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Patients.Features.SearchPatients;

/// <summary>
/// Recherche/filtrage des patients (nom, statut, praticien référent — cahier des charges §4.1).
/// </summary>
public record SearchPatientsQuery(
    string? SearchTerm = null,
    string? Status = null,
    Guid? ReferentPractitionerId = null,
    int Page = 1,
    int PageSize = 20
) : IQuery<Result<SearchPatientsResult>>;

public record SearchPatientsResult(
    IReadOnlyList<PatientSummaryDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record PatientSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly? BirthDate,
    string? Phone,
    string? Email,
    string Status,
    Guid? ReferentPractitionerId);
