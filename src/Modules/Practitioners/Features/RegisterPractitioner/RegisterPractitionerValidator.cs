using FluentValidation;

namespace OmniCare.Modules.Practitioners.Features.RegisterPractitioner;

public class RegisterPractitionerValidator : AbstractValidator<RegisterPractitionerCommand>
{
    public RegisterPractitionerValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProfessionCode).NotEmpty();
        RuleFor(x => x.InamiNumber).NotEmpty()
            .Matches(@"^[\d.\-\s]{11,16}$")
            .WithMessage("Le numéro INAMI du praticien doit contenir 11 chiffres.");
        RuleFor(x => x.CabinetId).NotEmpty();
    }
}
