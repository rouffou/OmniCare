using OmniCare.Modules.Billing.Domain.Entities;
using OmniCare.Modules.Billing.Domain.Events;
using OmniCare.Modules.Billing.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;
using Xunit;

namespace OmniCare.UnitTests.Billing;

public class InvoiceTests
{
    private static Invoice NewInvoice() => Invoice.CreateNew(
        Guid.NewGuid(), Guid.NewGuid(), InamiCode.Create("560011"), Amount.Create(25.50m), insuredAtIssue: true);

    [Fact]
    public void CreateNew_issues_invoice_and_raises_event()
    {
        var invoice = NewInvoice();
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(25.50m, invoice.Total.Value);
        var generated = Assert.Single(invoice.DomainEvents.OfType<InvoiceGeneratedEvent>());
        Assert.Equal(invoice.Id, generated.InvoiceId);
        Assert.Equal("560011", generated.InamiCode);
    }

    [Fact]
    public void CreateNew_requires_patient_practitioner_and_positive_amount()
    {
        var code = InamiCode.Create("560011");
        Assert.Throws<DomainException>(() =>
            Invoice.CreateNew(Guid.Empty, Guid.NewGuid(), code, Amount.Create(10m), true));
        Assert.Throws<DomainException>(() =>
            Invoice.CreateNew(Guid.NewGuid(), Guid.Empty, code, Amount.Create(10m), true));
        Assert.Throws<DomainException>(() =>
            Invoice.CreateNew(Guid.NewGuid(), Guid.NewGuid(), code, Amount.Zero, true));
    }

    [Fact]
    public void MarkAsPaid_records_payment_and_raises_event()
    {
        var invoice = NewInvoice();
        invoice.MarkAsPaid("Bancontact");
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal("Bancontact", invoice.PaymentMethod);
        Assert.NotNull(invoice.PaidOn);
        Assert.Single(invoice.DomainEvents.OfType<InvoicePaidEvent>());
    }

    [Fact]
    public void Paid_invoice_cannot_be_paid_again_or_cancelled()
    {
        var invoice = NewInvoice();
        invoice.MarkAsPaid("Cash");
        Assert.Throws<DomainException>(() => invoice.MarkAsPaid("Cash"));
        Assert.Throws<DomainException>(() => invoice.Cancel("test"));
    }

    [Fact]
    public void Cancel_records_reason_and_blocks_payment()
    {
        var invoice = NewInvoice();
        invoice.Cancel("Erreur de code INAMI");
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
        Assert.Equal("Erreur de code INAMI", invoice.CancellationReason);
        Assert.Throws<DomainException>(() => invoice.MarkAsPaid("Cash"));
    }
}

public class AmountTests
{
    [Fact]
    public void Create_rounds_to_cents_with_bankers_rounding()
    {
        Assert.Equal(25.12m, Amount.Create(25.125m).Value);
        Assert.Equal(25.14m, Amount.Create(25.135m).Value);
    }

    [Fact]
    public void Create_rejects_negative_values()
    {
        Assert.Throws<DomainException>(() => Amount.Create(-0.01m));
    }

    [Fact]
    public void Operators_add_and_subtract()
    {
        var total = Amount.Create(20m) + Amount.Create(5.5m);
        Assert.Equal(25.50m, total.Value);
        Assert.Throws<DomainException>(() => Amount.Create(5m) - Amount.Create(10m));
    }
}
