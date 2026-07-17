using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.ChangeAppointmentStatus;

public class ChangeAppointmentStatusValidator : AbstractValidator<ChangeAppointmentStatusCommand>
{
    public ChangeAppointmentStatusValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.Transition)
            .Must(t => AppointmentTransitions.All.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Transition inconnue. Valeurs : {string.Join(", ", AppointmentTransitions.All)}.");
    }
}
