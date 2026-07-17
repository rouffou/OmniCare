using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.DefineAppointmentType;

/// <summary>
/// Ajout d'un type d'acte planifiable au référentiel (durée par défaut configurable
/// par profession — cahier des charges §4.2/§4.6).
/// </summary>
public record DefineAppointmentTypeCommand(
    string Name,
    string ProfessionCode,
    int DefaultDurationMinutes
) : ICommand<Result<Guid>>, ITransactionalRequest;
