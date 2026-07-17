using FluentValidation;
using OmniCare.Modules.Patients.Domain.Entities;

namespace OmniCare.Modules.Patients.Features.RevokeConsent;

public class RevokeConsentValidator : AbstractValidator<RevokeConsentCommand>
{
    public RevokeConsentValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ConsentType)
            .Must(t => Enum.TryParse<ConsentType>(t, ignoreCase: true, out _))
            .WithMessage("Type de consentement inconnu. Valeurs : HealthDataProcessing, SharingWithCareCircle, ElectronicCommunication.");
    }
}
