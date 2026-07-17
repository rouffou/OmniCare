using FluentValidation;
using OmniCare.Modules.Patients.Domain.Entities;

namespace OmniCare.Modules.Patients.Features.SearchPatients;

public class SearchPatientsValidator : AbstractValidator<SearchPatientsQuery>
{
    public SearchPatientsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Status)
            .Must(s => Enum.TryParse<PatientStatus>(s, ignoreCase: true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.Status))
            .WithMessage("Statut inconnu. Valeurs possibles : Active, Archived.");
    }
}
