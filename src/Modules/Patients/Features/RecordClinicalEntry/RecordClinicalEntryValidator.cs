using FluentValidation;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Features.RecordClinicalEntry;

public class RecordClinicalEntryValidator : AbstractValidator<RecordClinicalEntryCommand>
{
    public RecordClinicalEntryValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.AuthorPractitionerId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Title).MaximumLength(200);
        RuleFor(x => x.ProfessionCode)
            .Must(code => HealthProfession.All.Any(p =>
                string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"Profession de santé inconnue : « {x.ProfessionCode} ».");
        RuleFor(x => x.EntryType)
            .Must(t => Enum.TryParse<ClinicalEntryType>(t, ignoreCase: true, out _))
            .WithMessage("Type d'entrée inconnu. Valeurs : Anamnesis, ClinicalAssessment, TreatmentPlan, SessionReport, Other.");
    }
}
