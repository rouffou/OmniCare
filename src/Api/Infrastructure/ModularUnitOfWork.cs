using Mediarq.UnitOfWork;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.Modules.Patients.Infrastructure.Persistence;

namespace OmniCare.Api.Infrastructure;

/// <summary>
/// Unit of work composite du monolithe modulaire : le UnitOfWorkBehavior de Mediarq
/// n'injecte qu'un seul IUnitOfWork alors que chaque module possède son DbContext
/// (enregistrer AddMediarqEntityFrameworkCore pour chaque contexte ferait gagner le
/// dernier). Ne committe que les contextes ayant des modifications ; une commande
/// ne touchant qu'un module ne paie donc que son propre contexte.
/// À revoir si une commande doit un jour écrire dans plusieurs modules atomiquement
/// (SQLite : pas de transaction distribuée entre fichiers — passer par l'Outbox).
/// </summary>
public sealed class ModularUnitOfWork : IUnitOfWork
{
    private readonly PatientsDbContext _patients;
    private readonly AgendaDbContext _agenda;
    private readonly BillingDbContext _billing;

    public ModularUnitOfWork(PatientsDbContext patients, AgendaDbContext agenda, BillingDbContext billing)
    {
        _patients = patients;
        _agenda = agenda;
        _billing = billing;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var written = 0;
        if (_patients.ChangeTracker.HasChanges())
            written += await _patients.SaveChangesAsync(cancellationToken);
        if (_agenda.ChangeTracker.HasChanges())
            written += await _agenda.SaveChangesAsync(cancellationToken);
        if (_billing.ChangeTracker.HasChanges())
            written += await _billing.SaveChangesAsync(cancellationToken);
        return written;
    }
}
