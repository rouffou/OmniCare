using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Billing.Features.GetInvoiceOwner;

/// <summary>Résout le PatientId propriétaire d'une facture — vérification d'appartenance
/// utilisée par le portail patient (ticket #39) avant de servir un document financier.</summary>
public record GetInvoiceOwnerQuery(Guid InvoiceId) : IQuery<Result<Guid>>;
