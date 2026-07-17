using OmniCare.Modules.Billing.Domain.Events;
using OmniCare.Modules.Billing.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Billing.Domain.Entities;

/// <summary>
/// Facture d'un acte de soin, basée sur la nomenclature INAMI (code générique
/// <see cref="ActCode"/>/<see cref="InamiCode"/> — le référentiel des codes valides
/// par profession est configurable, cahier des charges §4.6). Machine à états :
/// Issued → Paid, ou Issued → Cancelled. Toute modification est journalisée
/// (commandes auditées) — l'historique d'une facture émise est immuable.
/// </summary>
public sealed class Invoice : AggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid PractitionerId { get; private set; }
    public InamiCode Code { get; private set; }
    public Amount Total { get; private set; }

    /// <summary>Assurabilité vérifiée via MyCareNet au moment de l'émission.</summary>
    public bool InsuredAtIssue { get; private set; }

    public InvoiceStatus Status { get; private set; }
    public DateTimeOffset IssuedOn { get; private set; }
    public DateTimeOffset? PaidOn { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? CancellationReason { get; private set; }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private Invoice()
    {
    }
#pragma warning restore CS8618

    public static Invoice CreateNew(
        Guid patientId,
        Guid practitionerId,
        InamiCode code,
        Amount total,
        bool insuredAtIssue)
    {
        if (patientId == Guid.Empty)
            throw new DomainException("Une facture doit être rattachée à un patient.");
        if (practitionerId == Guid.Empty)
            throw new DomainException("Une facture doit être rattachée à un praticien.");
        if (total.Value <= 0)
            throw new DomainException("Une facture doit porter sur un montant strictement positif.");

        var invoice = new Invoice
        {
            PatientId = patientId,
            PractitionerId = practitionerId,
            Code = code,
            Total = total,
            InsuredAtIssue = insuredAtIssue,
            Status = InvoiceStatus.Issued,
            IssuedOn = DateTimeOffset.UtcNow,
        };
        invoice.Raise(new InvoiceGeneratedEvent(
            invoice.Id, patientId, practitionerId, code.Value, total.Value));
        return invoice;
    }

    public void MarkAsPaid(string paymentMethod)
    {
        if (Status != InvoiceStatus.Issued)
            throw new DomainException($"Impossible d'encaisser une facture au statut {Status}.");
        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new DomainException("Le moyen de paiement est obligatoire.");

        Status = InvoiceStatus.Paid;
        PaidOn = DateTimeOffset.UtcNow;
        PaymentMethod = paymentMethod.Trim();
        Raise(new InvoicePaidEvent(Id, PatientId));
    }

    public void Cancel(string? reason)
    {
        if (Status != InvoiceStatus.Issued)
            throw new DomainException($"Impossible d'annuler une facture au statut {Status}.");
        Status = InvoiceStatus.Cancelled;
        CancellationReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        Raise(new InvoiceCancelledEvent(Id, PatientId, CancellationReason));
    }
}

public enum InvoiceStatus
{
    Issued = 0,
    Paid = 1,
    Cancelled = 2,
}
