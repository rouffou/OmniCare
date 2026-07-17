using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Agenda.Features.GetCabinetSchedule;

/// <summary>
/// Agenda multi-praticiens (vue cabinet — cahier des charges §4.2) sur une plage
/// de dates, rendez-vous groupés par praticien.
/// </summary>
public record GetCabinetScheduleQuery(
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc
) : IQuery<Result<IReadOnlyList<PractitionerScheduleDto>>>;

public record PractitionerScheduleDto(
    Guid PractitionerId,
    IReadOnlyList<CabinetAppointmentDto> Appointments);

public record CabinetAppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid AppointmentTypeId,
    string? AppointmentTypeName,
    DateTimeOffset Start,
    DateTimeOffset End,
    string Status);
