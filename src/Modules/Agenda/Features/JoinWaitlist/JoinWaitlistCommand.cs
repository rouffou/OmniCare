using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.JoinWaitlist;

/// <summary>Inscription d'un patient en liste d'attente pour un créneau qui se libérerait (§4.2).</summary>
public record JoinWaitlistCommand(
    Guid PatientId,
    Guid PractitionerId,
    Guid AppointmentTypeId,
    DateTimeOffset RequestedFromUtc,
    DateTimeOffset RequestedToUtc,
    string? Notes
) : ICommand<Result<Guid>>, ITransactionalRequest;
