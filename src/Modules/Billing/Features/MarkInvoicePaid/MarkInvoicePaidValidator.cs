using FluentValidation;

namespace OmniCare.Modules.Billing.Features.MarkInvoicePaid;

public class MarkInvoicePaidValidator : AbstractValidator<MarkInvoicePaidCommand>
{
    public MarkInvoicePaidValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50);
    }
}
