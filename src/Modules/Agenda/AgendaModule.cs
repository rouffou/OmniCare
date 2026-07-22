using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniCare.Modules.Agenda.Features.CancelAppointment;
using OmniCare.Modules.Agenda.Features.ChangeAppointmentStatus;
using OmniCare.Modules.Agenda.Features.DefineAppointmentType;
using OmniCare.Modules.Agenda.Features.FulfillWaitlistEntry;
using OmniCare.Modules.Agenda.Features.DefineRoom;
using OmniCare.Modules.Agenda.Features.GetCabinetSchedule;
using OmniCare.Modules.Agenda.Features.GetPractitionerSchedule;
using OmniCare.Modules.Agenda.Features.GetWaitlist;
using OmniCare.Modules.Agenda.Features.JoinWaitlist;
using OmniCare.Modules.Agenda.Features.LeaveWaitlist;
using OmniCare.Modules.Agenda.Features.ListAppointmentTypes;
using OmniCare.Modules.Agenda.Features.PortalCancelMyAppointment;
using OmniCare.Modules.Agenda.Features.PortalGetMyAppointments;
using OmniCare.Modules.Agenda.Features.PortalScheduleMyAppointment;
using OmniCare.Modules.Agenda.Features.RescheduleAppointment;
using OmniCare.Modules.Agenda.Features.ScheduleAppointment;
using OmniCare.Modules.Agenda.Features.ScheduleAppointmentSeries;
using OmniCare.Modules.Agenda.Features.SendAppointmentReminders;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.Modules.Agenda.Infrastructure.Services;

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

        // Rappels SMS/email (ticket #27) : fournisseur non choisi (pas de compte à créer),
        // impl. de dev qui journalise — même principe que FakeMyCareNetService (Billing).
        services.AddSingleton<INotificationSender, FakeNotificationSender>();
        services.AddHostedService<AppointmentReminderBackgroundService>();

        return services;
    }

    public static IEndpointRouteBuilder MapAgendaModule(this IEndpointRouteBuilder app)
    {
        app.MapDefineAppointmentType();
        app.MapDefineRoom();
        app.MapScheduleAppointment();
        app.MapScheduleAppointmentSeries();
        app.MapRescheduleAppointment();
        app.MapCancelAppointment();
        app.MapChangeAppointmentStatus();
        app.MapGetPractitionerSchedule();
        app.MapGetCabinetSchedule();
        app.MapJoinWaitlist();
        app.MapLeaveWaitlist();
        app.MapFulfillWaitlistEntry();
        app.MapGetWaitlist();
        app.MapSendAppointmentReminders();
        app.MapListAppointmentTypes();
        app.MapPortalGetMyAppointments();
        app.MapPortalScheduleMyAppointment();
        app.MapPortalCancelAppointment();
        return app;
    }
}
