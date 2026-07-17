using FluentValidation;

namespace OmniCare.Modules.Billing.Features.CancelInvoice;

public class CancelInvoiceValidator : AbstractValidator<CancelInvoiceCommand>
{
    public CancelInvoiceValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
