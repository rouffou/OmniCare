using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.CancelAppointment;

public class CancelAppointmentValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
