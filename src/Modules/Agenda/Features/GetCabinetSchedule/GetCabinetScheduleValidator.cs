using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.GetCabinetSchedule;

public class GetCabinetScheduleValidator : AbstractValidator<GetCabinetScheduleQuery>
{
    public GetCabinetScheduleValidator()
    {
        RuleFor(x => x.ToUtc)
            .GreaterThan(x => x.FromUtc)
            .WithMessage("La fin de la plage doit être postérieure à son début.");
        RuleFor(x => x)
            .Must(x => x.ToUtc - x.FromUtc <= TimeSpan.FromDays(31))
            .WithMessage("La vue cabinet ne peut pas excéder 31 jours.");
    }
}
