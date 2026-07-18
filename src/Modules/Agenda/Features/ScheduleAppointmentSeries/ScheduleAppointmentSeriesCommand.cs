using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.ScheduleAppointmentSeries;

/// <summary>
/// Planification d'une série de séances récurrentes (cahier des charges §4.2),
/// hebdomadaire (ou multiple de semaines via <paramref name="IntervalWeeks"/>),
/// même créneau horaire répété <paramref name="OccurrenceCount"/> fois.
/// Tout-ou-rien : si une occurrence entre en conflit, aucune n'est créée.
/// </summary>
public record ScheduleAppointmentSeriesCommand(
    Guid PractitionerId,
    Guid PatientId,
    Guid AppointmentTypeId,
    DateTimeOffset FirstStartUtc,
    DateTimeOffset? EndUtc,
    int OccurrenceCount,
    int IntervalWeeks,
    string? Notes
) : ICommand<Result<AppointmentSeriesResult>>, ITransactionalRequest;

public record AppointmentSeriesResult(Guid SeriesId, IReadOnlyList<Guid> AppointmentIds);
