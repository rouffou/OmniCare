using FluentValidation;

namespace OmniCare.Modules.Patients.Features.RegisterPatient;

public class RegisterPatientValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NationalRegistryNumber)
            .Matches(@"^[\d.\- ]{11,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalRegistryNumber))
            .WithMessage("Le numéro de registre national doit contenir 11 chiffres.");
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.BirthDate)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.BirthDate.HasValue)
            .WithMessage("La date de naissance ne peut pas être dans le futur.");
        RuleFor(x => x.MutualityMemberNumber)
            .Empty()
            .When(x => string.IsNullOrWhiteSpace(x.MutualityCode))
            .WithMessage("Un numéro d'affilié requiert un code de mutuelle.");
    }
}
