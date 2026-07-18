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

    /// <summary>Statut BIM/OMNIO du patient au moment de l'émission (snapshot informatif,
    /// ne détermine aucun montant — la part patient reste un montant fourni par l'appelant).</summary>
    public bool PreferentialRateAtIssue { get; private set; }

    /// <summary>Part facturée à l'organisme assureur (tiers payant).</summary>
    public Amount MutualityShare { get; private set; }

    /// <summary>Part à charge du patient (ticket modérateur).</summary>
    public Amount PatientShare { get; private set; }

    /// <summary>Tiers payant appliqué : la part mutuelle est réclamée directement à
    /// l'organisme assureur, le patient ne règle que <see cref="PatientShare"/>.</summary>
    public bool ThirdPartyPayer { get; private set; }

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
        bool insuredAtIssue,
        bool preferentialRateAtIssue,
        Amount patientShare,
        bool thirdPartyPayer)
    {
        if (patientId == Guid.Empty)
            throw new DomainException("Une facture doit être rattachée à un patient.");
        if (practitionerId == Guid.Empty)
            throw new DomainException("Une facture doit être rattachée à un praticien.");
        if (total.Value <= 0)
            throw new DomainException("Une facture doit porter sur un montant strictement positif.");
        if (patientShare.Value > total.Value)
            throw new DomainException("La part patient ne peut pas dépasser le montant total de la facture.");

        var mutualityShare = total - patientShare;
        if (thirdPartyPayer && mutualityShare.Value <= 0)
            throw new DomainException("Le tiers payant suppose qu'une part soit réclamée à l'organisme assureur.");

        var invoice = new Invoice
        {
            PatientId = patientId,
            PractitionerId = practitionerId,
            Code = code,
            Total = total,
            InsuredAtIssue = insuredAtIssue,
            PreferentialRateAtIssue = preferentialRateAtIssue,
            PatientShare = patientShare,
            MutualityShare = mutualityShare,
            ThirdPartyPayer = thirdPartyPayer,
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
