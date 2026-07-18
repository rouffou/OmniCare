using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.FulfillWaitlistEntry;

public class FulfillWaitlistEntryValidator : AbstractValidator<FulfillWaitlistEntryCommand>
{
    public FulfillWaitlistEntryValidator()
    {
        RuleFor(x => x.WaitlistEntryId).NotEmpty();
    }
}
