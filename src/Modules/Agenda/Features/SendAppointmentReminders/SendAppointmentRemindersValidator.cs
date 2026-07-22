using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.SendAppointmentReminders;

public class SendAppointmentRemindersValidator : AbstractValidator<SendAppointmentRemindersCommand>
{
    public SendAppointmentRemindersValidator()
    {
        RuleFor(x => x.LeadTimeHours).GreaterThan(0);
    }
}
