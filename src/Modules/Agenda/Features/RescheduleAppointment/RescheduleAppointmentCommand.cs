using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.RescheduleAppointment;

/// <summary>
/// Déplacement d'un rendez-vous. Si <paramref name="EndUtc"/> est omis, la durée
/// par défaut du type d'acte s'applique. Le déplacement invalide la confirmation
/// (retour au statut Planned) et revérifie les chevauchements du praticien.
/// </summary>
public record RescheduleAppointmentCommand(
    Guid AppointmentId,
    DateTimeOffset StartUtc,
    DateTimeOffset? EndUtc
) : ICommand<Result<Guid>>, ITransactionalRequest;
