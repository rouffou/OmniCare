using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.LeaveWaitlist;

public class LeaveWaitlistValidator : AbstractValidator<LeaveWaitlistCommand>
{
    public LeaveWaitlistValidator()
    {
        RuleFor(x => x.WaitlistEntryId).NotEmpty();
    }
}
