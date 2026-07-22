using FluentValidation;

namespace OmniCare.Modules.Patients.Features.LinkPatientPortalAccount;

public class LinkPatientPortalAccountValidator : AbstractValidator<LinkPatientPortalAccountCommand>
{
    public LinkPatientPortalAccountValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PortalUserId).NotEmpty().MaximumLength(255);
    }
}
