using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Agenda.Features.GetPractitionerSchedule;

/// <summary>Agenda individuel d'un praticien sur une plage de dates (vue jour/semaine).</summary>
public record GetPractitionerScheduleQuery(
    Guid PractitionerId,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc
) : IQuery<Result<IReadOnlyList<AppointmentDto>>>;

public record AppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid AppointmentTypeId,
    string? AppointmentTypeName,
    DateTimeOffset Start,
    DateTimeOffset End,
    string Status,
    string? Notes);
