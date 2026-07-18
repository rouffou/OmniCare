using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.DefineRoom;

/// <summary>Ajout d'une salle/équipement au référentiel du cabinet (cahier des charges §4.2, optionnel).</summary>
public record DefineRoomCommand(string Name) : ICommand<Result<Guid>>, ITransactionalRequest;
