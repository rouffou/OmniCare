using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;

namespace OmniCare.Modules.Agenda.Features.SendAppointmentReminders;

/// <summary>
/// Envoie un rappel SMS/email pour chaque rendez-vous à venir dans les <paramref name="LeadTimeHours"/>
/// heures qui n'en a pas encore reçu (cahier des charges §4.2, ticket #27). Déclenché soit
/// périodiquement par <c>AppointmentReminderBackgroundService</c>, soit manuellement via l'endpoint
/// de la slice (ops/tests).
/// </summary>
public record SendAppointmentRemindersCommand(int LeadTimeHours) : ICommand<Result<int>>, ITransactionalRequest;
