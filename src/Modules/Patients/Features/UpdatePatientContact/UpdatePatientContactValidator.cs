using FluentValidation;

namespace OmniCare.Modules.Patients.Features.UpdatePatientContact;

public class UpdatePatientContactValidator : AbstractValidator<UpdatePatientContactCommand>
{
    public UpdatePatientContactValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.AddressLine).MaximumLength(300);
        RuleFor(x => x.PostalCode).MaximumLength(10);
        RuleFor(x => x.City).MaximumLength(100);
    }
}
