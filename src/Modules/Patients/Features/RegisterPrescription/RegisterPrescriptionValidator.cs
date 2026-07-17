using FluentValidation;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.RegisterPrescription;

public class RegisterPrescriptionValidator : AbstractValidator<RegisterPrescriptionCommand>
{
    public RegisterPrescriptionValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PrescriberName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SessionsPrescribed).InclusiveBetween(1, 120);
        RuleFor(x => x.PrescribedOn)
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("La date de prescription ne peut pas être dans le futur.");
        RuleFor(x => x.ProfessionCode)
            .Must(code => HealthProfession.All.Any(p =>
                string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"Profession de santé inconnue : « {x.ProfessionCode} ».");
    }
}
