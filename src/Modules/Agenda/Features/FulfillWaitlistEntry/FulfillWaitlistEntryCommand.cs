using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.FulfillWaitlistEntry;

/// <summary>
/// Clôture d'une inscription en liste d'attente une fois qu'un créneau libéré a été
/// réservé pour ce patient (via ScheduleAppointment, effectué séparément par le
/// secrétariat) — sort l'entrée de la liste active.
/// </summary>
public record FulfillWaitlistEntryCommand(Guid WaitlistEntryId) : ICommand<Result<Guid>>, ITransactionalRequest;
