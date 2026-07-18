using FluentValidation;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Billing.Features.DefineActCatalogEntry;

public class DefineActCatalogEntryValidator : AbstractValidator<DefineActCatalogEntryCommand>
{
    public DefineActCatalogEntryValidator()
    {
        RuleFor(x => x.CodeStr).NotEmpty().Matches(@"^\d{6}$")
            .WithMessage("Le code INAMI doit comporter 6 chiffres.");
        RuleFor(x => x.Label).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DefaultTariff).GreaterThan(0);
        RuleFor(x => x.ProfessionCode)
            .Must(code => HealthProfession.All.Any(p =>
                string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"Profession de santé inconnue : « {x.ProfessionCode} ».");
    }
}
