using FluentValidation;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Agenda.Features.DefineAppointmentType;

public class DefineAppointmentTypeValidator : AbstractValidator<DefineAppointmentTypeCommand>
{
    public DefineAppointmentTypeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DefaultDurationMinutes).InclusiveBetween(1, 480);
        RuleFor(x => x.ProfessionCode)
            .Must(code => HealthProfession.All.Any(p =>
                string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"Profession de santé inconnue : « {x.ProfessionCode} ».");
    }
}
