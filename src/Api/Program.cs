using FluentValidation;
using Mediarq.Extensions;
using Mediarq.FluentValidation;
using Mediarq.Outbox;
using Mediarq.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using OmniCare.Api.Infrastructure;
using OmniCare.Api.Infrastructure.Auditing;
using Mediarq.Idempotency;
using OmniCare.Modules.Agenda;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.Modules.Billing;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.Modules.Patients;
using OmniCare.Modules.Patients.Features.RegisterPatient;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.Modules.Practitioners;
using OmniCare.Modules.Practitioners.Features.RegisterCabinet;
using OmniCare.Modules.Practitioners.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Application.Auditing;
using OmniCare.SharedKernel.Application.Behaviors;

// Édition Community (gratuite sous 1M$ de revenu annuel, cf. Directory.Packages.props) —
// requis par QuestPDF avant toute génération de document (GetInvoicePdf, ticket #38).
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
    typeof(BillingModule).Assembly,
    typeof(RegisterCabinetCommand).Assembly,
]);
builder.Services.AddMediarqFluentValidation();

builder.Services.AddMediarq(
        isHttp: true,
        typeof(RegisterPatientCommand).Assembly,
        typeof(OmniCare.Modules.Agenda.AgendaModule).Assembly,
        typeof(BillingModule).Assembly,
        typeof(RegisterCabinetCommand).Assembly)
    .AddMediarqRequestLogging();

// Idempotence des commandes de facturation (IIdempotentRequest) — IDistributedCache
// mémoire en dev ; Redis en production multi-instances.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMediarqIdempotency();

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

// --- Authentification : JWT Bearer OIDC générique (ticket #26, cahier des charges §5.1) ---
// Indépendant du fournisseur (Authority/Audience configurables) — le MFA est délégué au
// fournisseur d'identité, OmniCare ne stocke jamais de mot de passe ni de secret TOTP.
// Authority vide tant qu'aucun fournisseur n'est choisi : la validation d'un token échoue
// alors simplement (une requête sans Authorization header n'est pas affectée). Infrastructure
// seule pour l'instant — aucun [Authorize]/RequireAuthorization n'est encore actif sur les
// endpoints métier, cf. /api/me pour la démonstration du câblage ; l'activation par endpoint
// est un ticket suivant, module par module.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Roles.Practitioner, p => p.RequireRole(Roles.Practitioner))
    .AddPolicy(Roles.Secretariat, p => p.RequireRole(Roles.Secretariat))
    .AddPolicy(Roles.Admin, p => p.RequireRole(Roles.Admin))
    .AddPolicy(Roles.Patient, p => p.RequireRole(Roles.Patient));

// --- Persistance (SQLite en dev ; hébergeur certifié données de santé en prod, §5.2) ---
// Une base par module : isolation des données conforme au découpage modulaire, et
// EnsureCreated fonctionne par contexte (il ne crée rien dans une base déjà peuplée).
builder.Services.AddDbContext<AuditDbContext>(o => o.UseSqlite(
    builder.Configuration.GetConnectionString("Audit") ?? "Data Source=omnicare-audit.db"));
builder.Services.AddPatientsModule(o => o.UseSqlite(
    builder.Configuration.GetConnectionString("Patients") ?? "Data Source=omnicare-patients.db"));
builder.Services.AddAgendaModule(o => o.UseSqlite(
    builder.Configuration.GetConnectionString("Agenda") ?? "Data Source=omnicare-agenda.db"));
builder.Services.AddBillingModule(o => o.UseSqlite(
    builder.Configuration.GetConnectionString("Billing") ?? "Data Source=omnicare-billing.db"));
builder.Services.AddPractitionersModule(o => o.UseSqlite(
    builder.Configuration.GetConnectionString("Practitioners") ?? "Data Source=omnicare-practitioners.db"));

// Outbox transactionnel pour InvoiceGeneratedEvent (télétransmission eAttest fiable —
// cf. GenerateInvoiceHandler). Intervalle court en dev pour un retour rapide ; à ajuster
// en production selon le volume de facturation.
builder.Services.AddMediarqOutbox<BillingDbContext>(o => o.PollingInterval = TimeSpan.FromSeconds(3));

var app = builder.Build();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

// Dev uniquement : application automatique des migrations EF Core au démarrage.
// En production, les migrations seront appliquées par le pipeline de déploiement
// (dotnet ef database update), jamais par l'application elle-même.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AuditDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<PatientsDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<AgendaDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<BillingDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<PractitionersDbContext>().Database.Migrate();
}

app.MapPatientsModule();
app.MapAgendaModule();
app.MapBillingModule();
app.MapPractitionersModule();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("HealthCheck");

// Démontre le câblage de l'authentification (ticket #26) : 401 sans token valide.
// Endpoint de diagnostic, pas une slice métier — pas de [Authorize(Roles=...)] spécifique,
// n'importe quel utilisateur authentifié peut voir sa propre identité.
app.MapGet("/api/me", (ICurrentUserService currentUser) =>
        Results.Ok(new { currentUser.UserId, currentUser.DisplayName, currentUser.Roles }))
    .RequireAuthorization()
    .WithName("Me");

app.Run();
