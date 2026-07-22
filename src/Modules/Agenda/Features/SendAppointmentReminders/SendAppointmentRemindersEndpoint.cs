using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace OmniCare.Modules.Agenda.Features.SendAppointmentReminders;

public static class SendAppointmentRemindersEndpoint
{
    /// <summary>
    /// Déclenchement manuel (ops/tests) du même traitement que <c>AppointmentReminderBackgroundService</c> —
    /// utile pour vérifier le câblage sans attendre le prochain cycle du job périodique.
    /// </summary>
    public static IEndpointRouteBuilder MapSendAppointmentReminders(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/reminders/run", async (
            ISender sender,
            IConfiguration configuration,
            CancellationToken ct) =>
        {
            var leadTimeHours = configuration.GetValue("Agenda:ReminderLeadTimeHours", 24);
            var result = await sender.Send(new SendAppointmentRemindersCommand(leadTimeHours), ct);
            return result.ToHttpResult();
        })
        .WithName("SendAppointmentReminders")
        .WithTags("Agenda");

        return app;
    }
}
