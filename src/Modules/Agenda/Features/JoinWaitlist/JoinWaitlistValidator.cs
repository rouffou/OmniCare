using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.JoinWaitlist;

public class JoinWaitlistValidator : AbstractValidator<JoinWaitlistCommand>
{
    public JoinWaitlistValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PractitionerId).NotEmpty();
        RuleFor(x => x.AppointmentTypeId).NotEmpty();
        RuleFor(x => x.RequestedFromUtc)
            .GreaterThan(_ => DateTimeOffset.UtcNow)
            .WithMessage("La période souhaitée doit commencer dans le futur.");
        RuleFor(x => x.RequestedToUtc)
            .GreaterThan(x => x.RequestedFromUtc)
            .WithMessage("La fin de la période souhaitée doit être postérieure à son début.");
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
