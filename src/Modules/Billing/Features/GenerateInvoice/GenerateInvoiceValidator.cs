using FluentValidation;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Billing.Features.GenerateInvoice;

public class GenerateInvoiceValidator : AbstractValidator<GenerateInvoiceCommand>
{
    public GenerateInvoiceValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PractitionerId).NotEmpty();
        RuleFor(x => x.InamiCodeStr).NotEmpty().Matches(@"^\d{6}$")
            .WithMessage("Le code INAMI doit comporter 6 chiffres.");
        RuleFor(x => x.BaseAmount).GreaterThan(0);
        RuleFor(x => x.IdempotencyKey).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProfessionCode)
            .Must(code => HealthProfession.All.Any(p =>
                string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"Profession de santé inconnue : « {x.ProfessionCode} ».");
    }
}
