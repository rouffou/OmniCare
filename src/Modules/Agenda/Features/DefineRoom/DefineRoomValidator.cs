using FluentValidation;

namespace OmniCare.Modules.Agenda.Features.DefineRoom;

public class DefineRoomValidator : AbstractValidator<DefineRoomCommand>
{
    public DefineRoomValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
