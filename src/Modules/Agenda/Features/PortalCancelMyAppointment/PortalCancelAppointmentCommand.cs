using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.PortalCancelMyAppointment;

/// <summary>
/// Annulation d'un rendez-vous par le patient lui-même (portail, ticket #39). Contrairement
/// à <c>CancelAppointmentCommand</c> (accès praticien/secrétariat, aucune vérification
/// d'appartenance), vérifie que le rendez-vous appartient bien au patient authentifié.
/// </summary>
public record PortalCancelAppointmentCommand(Guid AppointmentId, Guid PatientId, string? Reason)
    : ICommand<Result<Guid>>, ITransactionalRequest;
