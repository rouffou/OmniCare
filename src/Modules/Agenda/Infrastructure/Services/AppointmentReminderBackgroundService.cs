using Mediarq.Core.Mediators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OmniCare.Modules.Agenda.Features.SendAppointmentReminders;

namespace OmniCare.Modules.Agenda.Infrastructure.Services;

/// <summary>
/// Scanne périodiquement les rendez-vous à venir et déclenche l'envoi des rappels dus
/// (cahier des charges §4.2, ticket #27). Premier job récurrent du repo : pas de
/// Quartz/Hangfire, un simple <see cref="BackgroundService"/> suffit à ce volume.
/// </summary>
public sealed class AppointmentReminderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AppointmentReminderBackgroundService> _logger;

    public AppointmentReminderBackgroundService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<AppointmentReminderBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var leadTimeHours = _configuration.GetValue("Agenda:ReminderLeadTimeHours", 24);
        var pollingIntervalMinutes = _configuration.GetValue("Agenda:ReminderPollingIntervalMinutes", 15);
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(pollingIntervalMinutes));

        do
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                var result = await sender.Send(
                    new SendAppointmentRemindersCommand(leadTimeHours), stoppingToken);
                if (result.IsSuccess && result.Value > 0)
                    _logger.LogInformation("{Count} rappel(s) de rendez-vous envoyé(s).", result.Value);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Un échec ponctuel (ex. base momentanément indisponible) ne doit pas arrêter
                // le job — le prochain cycle réessaiera sur les rendez-vous toujours dus.
                _logger.LogError(ex, "Échec du cycle d'envoi des rappels de rendez-vous.");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
