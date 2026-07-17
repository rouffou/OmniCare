using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.RescheduleAppointment;

public class RescheduleAppointmentValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.StartUtc)
            .GreaterThan(_ => DateTimeOffset.UtcNow)
            .WithMessage("Un rendez-vous doit être déplacé vers un créneau futur.");
        RuleFor(x => x.EndUtc)
            .GreaterThan(x => x.StartUtc)
            .When(x => x.EndUtc.HasValue)
            .WithMessage("La fin du rendez-vous doit être postérieure à son début.");
    }
}
