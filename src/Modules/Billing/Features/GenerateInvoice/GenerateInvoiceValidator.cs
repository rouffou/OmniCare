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
        RuleFor(x => x.PatientShareAmount)
            .InclusiveBetween(0, decimal.MaxValue)
            .LessThanOrEqualTo(x => x.BaseAmount)
            .When(x => x.PatientShareAmount.HasValue)
            .WithMessage("La part patient doit être comprise entre 0 et le montant total.");
        When(x => x.ThirdPartyPayer, () =>
        {
            RuleFor(x => x.PatientShareAmount)
                .NotNull()
                .LessThan(x => x.BaseAmount)
                .WithMessage("Le tiers payant suppose une part patient inférieure au montant total.");
        });
        RuleFor(x => x.ProfessionCode)
            .Must(code => HealthProfession.All.Any(p =>
                string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"Profession de santé inconnue : « {x.ProfessionCode} ».");
    }
}
