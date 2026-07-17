using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.GetPractitionerSchedule;

public class GetPractitionerScheduleValidator : AbstractValidator<GetPractitionerScheduleQuery>
{
    public GetPractitionerScheduleValidator()
    {
        RuleFor(x => x.PractitionerId).NotEmpty();
        RuleFor(x => x.ToUtc)
            .GreaterThan(x => x.FromUtc)
            .WithMessage("La fin de la plage doit être postérieure à son début.");
        RuleFor(x => x)
            .Must(x => x.ToUtc - x.FromUtc <= TimeSpan.FromDays(62))
            .WithMessage("La plage consultée ne peut pas excéder 62 jours.");
    }
}
