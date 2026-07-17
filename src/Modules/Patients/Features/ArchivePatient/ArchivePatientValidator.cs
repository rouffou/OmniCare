using FluentValidation;

namespace OmniCare.Modules.Patients.Features.ArchivePatient;

public class ArchivePatientValidator : AbstractValidator<ArchivePatientCommand>
{
    public ArchivePatientValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
    }
}
