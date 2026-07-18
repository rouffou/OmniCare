using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Billing.Features.DefineActCatalogEntry;

/// <summary>Ajout d'un acte facturable au référentiel d'une profession (cahier des charges §4.6).</summary>
public record DefineActCatalogEntryCommand(
    string ProfessionCode,
    string CodeStr,
    string Label,
    decimal DefaultTariff
) : ICommand<Result<Guid>>, ITransactionalRequest;
