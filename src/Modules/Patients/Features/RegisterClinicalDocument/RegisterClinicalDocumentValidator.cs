using FluentValidation;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.RegisterClinicalDocument;

public class RegisterClinicalDocumentValidator : AbstractValidator<RegisterClinicalDocumentCommand>
{
    public RegisterClinicalDocumentValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProfessionCode)
            .Must(code => HealthProfession.All.Any(p =>
                string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"Profession de santé inconnue : « {x.ProfessionCode} ».");
        RuleFor(x => x.DocumentType)
            .Must(type => Enum.TryParse<ClinicalDocumentType>(type, ignoreCase: true, out _))
            .WithMessage(x => $"Type de document inconnu : « {x.DocumentType} ».");
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ContentBase64).NotEmpty();
        RuleFor(x => x.UploadedByPractitionerId).NotEmpty();
    }
}
