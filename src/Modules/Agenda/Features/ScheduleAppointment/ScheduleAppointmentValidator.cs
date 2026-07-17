using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.ScheduleAppointment;

public class ScheduleAppointmentValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentValidator()
    {
        RuleFor(x => x.PractitionerId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.AppointmentTypeId).NotEmpty();
        RuleFor(x => x.StartUtc)
            .GreaterThan(_ => DateTimeOffset.UtcNow)
            .WithMessage("Un rendez-vous doit être planifié dans le futur.");
        RuleFor(x => x.EndUtc)
            .GreaterThan(x => x.StartUtc)
            .When(x => x.EndUtc.HasValue)
            .WithMessage("La fin du rendez-vous doit être postérieure à son début.");
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
