using FluentValidation;
using Mediarq.Extensions;
using Mediarq.FluentValidation;
using Mediarq.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using OmniCare.Api.Infrastructure;
using OmniCare.Api.Infrastructure.Auditing;
using OmniCare.Modules.Agenda;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.Modules.Patients;
using OmniCare.Modules.Patients.Features.RegisterPatient;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Application.Auditing;
using OmniCare.SharedKernel.Application.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

// --- Mediarq : pipeline (cf. architecture_technique_saas.md §5) :
// Logging → Validation (auto si un IValidator<T> est enregistré) → AuditBehavior custom.
// L'adaptateur FluentValidation DOIT être enregistré avant AddMediarq (scan) : sinon le
// scan ne voit aucun IValidator<> et la validation ne fait silencieusement rien
// (cf. README Mediarq.FluentValidation, section Registration order).
builder.Services.AddValidatorsFromAssemblies(
[
    typeof(RegisterPatientCommand).Assembly,
    typeof(OmniCare.Modules.Agenda.AgendaModule).Assembly,
]);
builder.Services.AddMediarqFluentValidation();

builder.Services.AddMediarq(
        isHttp: true,
        typeof(RegisterPatientCommand).Assembly,
        typeof(OmniCare.Modules.Agenda.AgendaModule).Assembly)
    .AddMediarqRequestLogging();

// Unit of Work : les commandes marquées ITransactionalRequest sont commitées par le
// UnitOfWorkBehavior après le handler, uniquement si le Result est un succès — les
// handlers ne font plus de SaveChangesAsync. Implémentation composite car un DbContext
// par module (cf. ModularUnitOfWork).
builder.Services.AddMediarqUnitOfWork<ModularUnitOfWork>();

// Behavior d'audit custom (aucun behavior natif — cf. §5 du doc d'architecture).
builder.Services.AddScoped(
    typeof(Mediarq.Core.Common.Pipeline.IPipelineBehavior<,>), typeof(AuditBehavior<,>));

// --- Services transverses ---
builder.Services.AddScoped<ICurrentUserService, HttpCurrentUserService>();
builder.Services.AddScoped<IAuditTrailStore, EfCoreAuditTrailStore>();

// --- Persistance (SQLite en dev ; hébergeur certifié données de santé en prod, §5.2) ---
// Une base par module : isolation des données conforme au découpage modulaire, et
// EnsureCreated fonctionne par contexte (il ne crée rien dans une base déjà peuplée).
builder.Services.AddDbContext<AuditDbContext>(o => o.UseSqlite(
    builder.Configuration.GetConnectionString("Audit") ?? "Data Source=omnicare-audit.db"));
builder.Services.AddPatientsModule(o => o.UseSqlite(
    builder.Configuration.GetConnectionString("Patients") ?? "Data Source=omnicare-patients.db"));
builder.Services.AddAgendaModule(o => o.UseSqlite(
    builder.Configuration.GetConnectionString("Agenda") ?? "Data Source=omnicare-agenda.db"));

var app = builder.Build();

app.UseExceptionHandler();

// Dev uniquement : création du schéma sans migrations. À remplacer par des
// migrations EF Core avant tout déploiement.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AuditDbContext>().Database.EnsureCreated();
    scope.ServiceProvider.GetRequiredService<PatientsDbContext>().Database.EnsureCreated();
    scope.ServiceProvider.GetRequiredService<AgendaDbContext>().Database.EnsureCreated();
}

app.MapPatientsModule();
app.MapAgendaModule();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("HealthCheck");

app.Run();
