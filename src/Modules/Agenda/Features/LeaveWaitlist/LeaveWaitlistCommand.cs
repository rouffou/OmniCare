using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.LeaveWaitlist;

/// <summary>Retrait volontaire d'une inscription en liste d'attente.</summary>
public record LeaveWaitlistCommand(Guid WaitlistEntryId) : ICommand<Result<Guid>>, ITransactionalRequest;
