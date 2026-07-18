using FluentValidation;

namespace OmniCare.Modules.Billing.Features.RetryInvoiceTransmission;

public class RetryInvoiceTransmissionValidator : AbstractValidator<RetryInvoiceTransmissionCommand>
{
    public RetryInvoiceTransmissionValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
    }
}
