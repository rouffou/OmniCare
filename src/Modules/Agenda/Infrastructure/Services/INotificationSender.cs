using Microsoft.Extensions.Logging;

namespace OmniCare.Modules.Agenda.Infrastructure.Services;

public enum NotificationChannel
{
    Email = 0,
    Sms = 1,
}

public sealed record NotificationMessage(NotificationChannel Channel, string Recipient, string Subject, string Body);

/// <summary>
/// Abstraction de l'envoi de rappels SMS/email (cahier des charges §4.2, ticket #27),
/// indépendante du fournisseur retenu (à souscrire séparément — SendGrid/Twilio/autre).
/// </summary>
public interface INotificationSender
{
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implémentation de développement : journalise le message au lieu de l'envoyer réellement.
/// Le client réel (fournisseur SMS/email) sera branchable ici sans toucher aux handlers,
/// même principe que <c>FakeMyCareNetService</c> (module Billing).
/// </summary>
public sealed class FakeNotificationSender : INotificationSender
{
    private readonly ILogger<FakeNotificationSender> _logger;

    public FakeNotificationSender(ILogger<FakeNotificationSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[FakeNotificationSender] {Channel} → {Recipient} : {Subject} — {Body}",
            message.Channel, message.Recipient, message.Subject, message.Body);
        return Task.CompletedTask;
    }
}
