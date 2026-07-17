using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.ChangeAppointmentStatus;

/// <summary>
/// Transition de statut d'un rendez-vous : Confirm, Complete ou NoShow
/// (l'annulation a sa propre slice, avec motif).
/// </summary>
public record ChangeAppointmentStatusCommand(Guid AppointmentId, string Transition) : ICommand<Result<Guid>>, ITransactionalRequest;

public static class AppointmentTransitions
{
    public const string Confirm = "Confirm";
    public const string Complete = "Complete";
    public const string NoShow = "NoShow";

    public static readonly IReadOnlyList<string> All = [Confirm, Complete, NoShow];
}
