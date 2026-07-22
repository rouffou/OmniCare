namespace OmniCare.Modules.Billing.Features.GetInvoicePdf;

/// <summary>Données assemblées pour la mise en page — le <see cref="InvoicePdfBuilder"/>
/// n'accède à aucune source de données, il ne fait que composer le document.</summary>
public sealed record InvoicePdfData(
    Guid InvoiceId,
    DateTimeOffset IssuedOn,
    string ActLabel,
    string ActCode,
    decimal Total,
    decimal PatientShare,
    decimal MutualityShare,
    bool ThirdPartyPayer,
    string Status,
    DateTimeOffset? PaidOn,
    string? PaymentMethod,
    string PractitionerName,
    string ProfessionLabel,
    string PractitionerInamiNumber,
    string CabinetName,
    string CabinetBceNumber,
    string CabinetAddressLine,
    string CabinetPostalCode,
    string CabinetCity,
    string PatientName,
    string? PatientAddressLine,
    string? PatientPostalCode,
    string? PatientCity);
