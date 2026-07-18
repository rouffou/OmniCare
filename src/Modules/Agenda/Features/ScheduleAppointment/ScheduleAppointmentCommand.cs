using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.ScheduleAppointment;

/// <summary>
/// Planification d'un rendez-vous. Si <paramref name="EndUtc"/> est omis, la durée
/// par défaut du type d'acte s'applique. Vérifie l'absence de chevauchement dans
/// l'agenda du praticien.
/// </summary>
public record ScheduleAppointmentCommand(
    Guid PractitionerId,
    Guid PatientId,
    Guid AppointmentTypeId,
    DateTimeOffset StartUtc,
    DateTimeOffset? EndUtc,
    string? Notes,
    Guid? RoomId = null
) : ICommand<Result<Guid>>, ITransactionalRequest;
