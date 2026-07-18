using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniCare.Modules.Agenda.Features.CancelAppointment;
using OmniCare.Modules.Agenda.Features.ChangeAppointmentStatus;
using OmniCare.Modules.Agenda.Features.DefineAppointmentType;
using OmniCare.Modules.Agenda.Features.GetCabinetSchedule;
using OmniCare.Modules.Agenda.Features.GetPractitionerSchedule;
using OmniCare.Modules.Agenda.Features.RescheduleAppointment;
using OmniCare.Modules.Agenda.Features.ScheduleAppointment;
using OmniCare.Modules.Agenda.Features.ScheduleAppointmentSeries;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;

namespace OmniCare.Modules.Agenda;

/// <summary>Composition root du module : persistance + endpoints des slices.</summary>
public static class AgendaModule
{
    public static IServiceCollection AddAgendaModule(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<AgendaDbContext>(configureDb);
        services.AddScoped<IAgendaDbContext>(sp => sp.GetRequiredService<AgendaDbContext>());
        return services;
    }

    public static IEndpointRouteBuilder MapAgendaModule(this IEndpointRouteBuilder app)
    {
        app.MapDefineAppointmentType();
        app.MapScheduleAppointment();
        app.MapScheduleAppointmentSeries();
        app.MapRescheduleAppointment();
        app.MapCancelAppointment();
        app.MapChangeAppointmentStatus();
        app.MapGetPractitionerSchedule();
        app.MapGetCabinetSchedule();
        return app;
    }
}
