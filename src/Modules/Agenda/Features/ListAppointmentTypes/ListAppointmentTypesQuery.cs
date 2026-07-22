using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Agenda.Features.ListAppointmentTypes;

/// <summary>Référentiel des types de rendez-vous actifs (cahier des charges §4.2/§4.6) —
/// utilisé notamment par le portail patient (ticket #39) pour choisir un motif de RDV.</summary>
public record ListAppointmentTypesQuery(string? ProfessionCode)
    : IQuery<Result<IReadOnlyList<AppointmentTypeDto>>>;

public record AppointmentTypeDto(
    Guid Id, string Name, string ProfessionCode, int DefaultDurationMinutes);
