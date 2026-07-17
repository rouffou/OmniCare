using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.CancelAppointment;

public record CancelAppointmentCommand(Guid AppointmentId, string? Reason) : ICommand<Result<Guid>>, ITransactionalRequest;
