using System.Globalization;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.Modules.Agenda.Infrastructure.Services;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.SendAppointmentReminders;

public class SendAppointmentRemindersHandler : ICommandHandler<SendAppointmentRemindersCommand, Result<int>>
{
    private readonly IAgendaDbContext _context;
    private readonly IPatientDirectory _patientDirectory;
    private readonly INotificationSender _notificationSender;

    public SendAppointmentRemindersHandler(
        IAgendaDbContext context, IPatientDirectory patientDirectory, INotificationSender notificationSender)
    {
        _context = context;
        _patientDirectory = patientDirectory;
        _notificationSender = notificationSender;
    }

    public async Task<Result<int>> Handle(
        SendAppointmentRemindersCommand request, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var horizon = now.AddHours(request.LeadTimeHours);

        var dueAppointments = await _context.Appointments
            .Where(a => a.ReminderSentOn == null
                && (a.Status == AppointmentStatus.Planned || a.Status == AppointmentStatus.Confirmed)
                && a.Slot.Start > now
                && a.Slot.Start <= horizon)
            .ToListAsync(cancellationToken);

        var sentCount = 0;
        foreach (var appointment in dueAppointments)
        {
            // Consentement ElectronicCommunication requis (RGPD art. 9) : pas de rappel sans
            // opt-in explicite du patient, même si un email/téléphone est renseigné.
            var identity = await _patientDirectory.GetIdentityAsync(appointment.PatientId, cancellationToken);
            if (identity is null || !identity.HasElectronicCommunicationConsent)
                continue;

            NotificationChannel channel;
            string recipient;
            if (identity.Email is not null)
            {
                channel = NotificationChannel.Email;
                recipient = identity.Email;
            }
            else if (identity.Phone is not null)
            {
                channel = NotificationChannel.Sms;
                recipient = identity.Phone;
            }
            else
            {
                continue;
            }

            var message = new NotificationMessage(
                channel,
                recipient,
                "Rappel de rendez-vous",
                $"Bonjour {identity.FullName}, rappel de votre rendez-vous le " +
                $"{appointment.Slot.Start.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}.");

            await _notificationSender.SendAsync(message, cancellationToken);
            appointment.MarkReminderSent();
            sentCount++;
        }

        return Result.Success(sentCount);
    }
}
