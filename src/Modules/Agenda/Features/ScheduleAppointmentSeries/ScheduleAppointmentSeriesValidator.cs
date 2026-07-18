using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.ScheduleAppointmentSeries;

public class ScheduleAppointmentSeriesValidator : AbstractValidator<ScheduleAppointmentSeriesCommand>
{
    public ScheduleAppointmentSeriesValidator()
    {
        RuleFor(x => x.PractitionerId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.AppointmentTypeId).NotEmpty();
        RuleFor(x => x.FirstStartUtc)
            .GreaterThan(_ => DateTimeOffset.UtcNow)
            .WithMessage("La première occurrence doit être planifiée dans le futur.");
        RuleFor(x => x.EndUtc)
            .GreaterThan(x => x.FirstStartUtc)
            .When(x => x.EndUtc.HasValue)
            .WithMessage("La fin du rendez-vous doit être postérieure à son début.");
        RuleFor(x => x.OccurrenceCount).InclusiveBetween(2, 52)
            .WithMessage("Une série comporte entre 2 et 52 occurrences.");
        RuleFor(x => x.IntervalWeeks).InclusiveBetween(1, 4)
            .WithMessage("L'intervalle entre occurrences est de 1 à 4 semaines.");
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
