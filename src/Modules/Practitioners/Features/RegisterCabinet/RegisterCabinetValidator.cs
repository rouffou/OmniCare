using FluentValidation;

namespace OmniCare.Modules.Practitioners.Features.RegisterCabinet;

public class RegisterCabinetValidator : AbstractValidator<RegisterCabinetCommand>
{
    public RegisterCabinetValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BceNumber).NotEmpty()
            .Matches(@"^[\d.\-\s]{10,15}$")
            .WithMessage("Le numéro d'entreprise (BCE) doit contenir 10 chiffres.");
        RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
    }
}
