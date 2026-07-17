# Spécifications de l'Architecture Technique SaaS
## Architecture Clean + Vertical Slice, DDD & CQRS avec Mediarq
**Cible :** Backend .NET Core pour la plateforme SaaS Kinésithérapie

---

## 1. Principes Fondateurs de l'Architecture

Pour garantir la pérennité, la testabilité et la capacité à évoluer du SaaS face aux futures réglementations eHealth, l'architecture repose sur l'alliance de trois grands paradigmes :

1. **Vertical Slice Architecture (Slices) :** Au lieu de découper le code par couches techniques horizontales (Controllers, Services, Repositories), le code est organisé par **fonctionnalités métiers** (ex: *AccepterUnRendezVous*, *GenererFactureINAMI*). Chaque slice encapsule sa propre logique de bout en bout.
2. **Clean Architecture & Domain-Driven Design (DDD) :** Le cœur du système (le Domaine) est totalement isolé des infrastructures externes (Bases de données, APIs eHealth). Les règles métiers de la kinésithérapie sont protégées contre les changements technologiques.
3. **CQRS (Command Query Responsibility Segregation) via Mediarq :** Séparation stricte entre les opérations de modification de l'état (Commandes) et les opérations de lecture (Requêtes), orchestrées par le médiateur en-mémoire **Mediarq**.

### 1.1 Choix du médiateur : Mediarq (plutôt que MediatR)

Le projet utilise **[Mediarq](https://www.nuget.org/packages/Mediarq/)**, une librairie CQRS légère, sans dépendance, pensée comme alternative libre à MediatR. Points clés à connaître avant de coder :

- **Commands / Queries avec `Result`** : les handlers renvoient un type `Result<T>` (programmation orientée rail — *railway-oriented programming*) plutôt que de lever des exceptions pour les cas d'échec métier attendus.
- **Commandes sans résultat (void)** : passent par le même pipeline via `ICommand` / `ICommandHandler<TCommand>` (réponse de type `Unit`).
- **Notifications** : équivalent des Domain Events MediatR, via `INotification` / `INotificationHandler<T>`, publiées à zéro ou plusieurs handlers.
- **Requêtes en streaming** : `IStreamRequest<T>` → `IAsyncEnumerable<T>`.
- **Pipeline de Behaviors composable et "lean by default"** : un behavior ne s'active que s'il y a effectivement quelque chose à faire (ex. la validation ne s'exécute que si un validateur est enregistré pour la requête) — un idle pipeline ne coûte rien.
- **Dispatch sans réflexion (source generator)** : compatible Native AOT / trimming via `AddMediarqCore()` + `AddMediarqHandlers()` généré à la compilation. L'enregistrement par scan (`AddMediarq(...)`) reste disponible et plus simple, mais repose sur la réflexion.
- **Scaffolding via template `dotnet new`** : `Mediarq.Templates` génère une Command + son Handler + son Validator en une seule commande (`dotnet new mediarq-feature`).

### 1.2 Packages NuGet à installer

Mediarq est distribué en plusieurs packages ; installer uniquement ce dont chaque module a besoin.

| Package | Rôle | Usage prévu dans OmniCare |
|---|---|---|
| `Mediarq` (meta-package) | Bundle `Mediarq.Core` + extensions légères (ASP.NET Core, FluentValidation, DataAnnotations, Caching, Diagnostics, UnitOfWork) | Référence par défaut dans chaque module `Features/` |
| `Mediarq.Core` | Médiateur, pipeline, `Result`, source generator | Inclus dans le meta-package `Mediarq` |
| `Mediarq.AspNetCore` | Mappe `Result`/`ResultError` → `IResult` + ProblemDetails (RFC 7807) | Endpoints Minimal API (retour homogène des erreurs métier) |
| `Mediarq.FluentValidation` | Exécute les validateurs FluentValidation dans le pipeline Mediarq | Chaque `<UseCase>Validator.cs` de slice |
| `Mediarq.DataAnnotations` | Validation via attributs `System.ComponentModel.DataAnnotations` | Optionnel, pour des DTOs simples hors Domain |
| `Mediarq.UnitOfWork` | Commit d'une unité de travail autour des commandes marquées `ITransactionalRequest` | Comportement "Transaction/Unit of Work" du pipeline (section 5) |
| `Mediarq.EntityFrameworkCore` | `EfCoreUnitOfWork<TContext>` branché sur un `DbContext` EF Core | Persistance de chaque module (`PatientsDbContext`, `BillingDbContext`, …) |
| `Mediarq.Caching` | Mémoïsation des réponses de requêtes marquées `ICacheableRequest` (`IMemoryCache` ou `IDistributedCache`/Redis) | Requêtes de lecture à fort volume (ex. disponibilités agenda) |
| `Mediarq.Idempotency` | Exécute une requête `IIdempotentRequest` au plus une fois par clé, rejoue le résultat stocké | Sécuriser les commandes de facturation/télétransmission contre les doublons |
| `Mediarq.Outbox` | Outbox transactionnel au-dessus d'EF Core (notifications mises en file puis publiées de façon fiable) | Fiabiliser les Domain Events critiques (ex. `InvoiceGenerated` → envoi eAttest) |
| `Mediarq.Diagnostics` | Traces `Activity` + métriques (compatible OpenTelemetry) | Observabilité transverse |
| `Mediarq.OpenTelemetry` | `AddMediarqInstrumentation()` sur les tracer/meter providers | Intégration avec la stack d'observabilité du SaaS |
| `Mediarq.Polly` | Retry / timeout / circuit breaker pour les requêtes `IResilientRequest` (via Polly) | Appels vers MyCareNet/eHealth (réseaux externes peu fiables) |
| `Mediarq.MassTransit` | Relaie les notifications vers un bus MassTransit, hors-process | À évaluer si un besoin d'événementiel inter-services apparaît (hors périmètre Phase 1) |
| `Mediarq.Templates` | Templates `dotnet new` pour scaffolder une Command + Handler + Validator | Outil de productivité pour créer une nouvelle slice |

Pour la Phase 1, l'ensemble minimal recommandé est : `Mediarq` (meta-package), `Mediarq.EntityFrameworkCore`, `Mediarq.Diagnostics`. Les autres packages s'ajoutent à la demande, module par module.

---

## 2. Structure d'un Projet & Découpage des Slices

L'application est divisée en modules logiques (ex: `Patients`, `Agenda`, `Billing`). Au sein d'un module, chaque cas d'utilisation est une "Tranche Verticale" (Vertical Slice).

### Structure des dossiers type d'un module :
```text
src/Modules/Billing/
│
├── Domain/                         <-- Coeur DDD (Isolé)
│   ├── Entities/                   - Invoice.cs, PatientSnapshot.cs
│   ├── ValueObjects/               - InamiCode.cs, Amount.cs
│   ├── Events/                     - InvoiceGeneratedEvent.cs
│   └── Exceptions/                 - InvalidInamiCodeException.cs
│
├── Features/                       <-- Slices Verticales (CQRS)
│   ├── GenerateInvoice/            
│   │   ├── GenerateInvoiceCommand.cs       - Le modèle de données reçu (ICommand<Result<T>>)
│   │   ├── GenerateInvoiceValidator.cs     - Validation FluentValidation (exécutée via Mediarq.FluentValidation)
│   │   ├── GenerateInvoiceHandler.cs       - L'exécuteur (ICommandHandler, Mediarq)
│   │   └── GenerateInvoiceEndpoint.cs      - Le point d'entrée HTTP Minimal API
│   │
│   ├── GetInvoiceById/
│   │   ├── GetInvoiceByIdQuery.cs
│   │   ├── GetInvoiceByIdHandler.cs
│   │   └── InvoiceDto.cs
│
├── Infrastructure/                 <-- Détails techniques
│   ├── Persistence/                - BillingDbContext.cs, Configurations EF Core
│   └── Services/                   - MyCareNetBillingClient.cs (Appels API)
```

---

## 3. Implémentation CQRS avec Mediarq

L'ensemble des interactions avec le système passe par le médiateur Mediarq, découplant les points d'entrée HTTP de la logique métier.

### 3.1 Exemple d'une Commande (Écriture) : `GenerateInvoice`

```csharp
using Mediarq;
using FluentValidation;

namespace SaaS.Modules.Billing.Features.GenerateInvoice;

// 1. La Commande (Data) — retourne un Result<Guid> (railway-oriented, pas d'exception pour un échec métier attendu)
public record GenerateInvoiceCommand(
    Guid PatientId, 
    string InamiCodeStr, 
    decimal BaseAmount
) : ICommand<Result<Guid>>;

// 2. Le Validateur (Sécurité à l'entrée) — exécuté automatiquement par le ValidationBehavior de Mediarq
//    dès lors qu'un IValidator<GenerateInvoiceCommand> est enregistré (via Mediarq.FluentValidation)
public class GenerateInvoiceValidator : AbstractValidator<GenerateInvoiceCommand>
{
    public GenerateInvoiceValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.InamiCodeStr).NotEmpty().Matches(@"^\d{6}$");
        RuleFor(x => x.BaseAmount).GreaterThan(0);
    }
}

// 3. Le Handler (Logique de la Slice + DDD)
public class GenerateInvoiceHandler : ICommandHandler<GenerateInvoiceCommand, Result<Guid>>
{
    private readonly IBillingDbContext _context;
    private readonly IMyCareNetService _myCareNet; // Abstraction eHealth

    public GenerateInvoiceHandler(IBillingDbContext context, IMyCareNetService myCareNet)
    {
        _context = context;
        _myCareNet = myCareNet;
    }

    public async Task<Result<Guid>> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken = default)
    {
        // Validation métier via des Value Objects du Domaine
        var inamiCode = InamiCode.Create(request.InamiCodeStr);
        
        // Appel infrastructure via interface pour l'assurabilité eHealth
        var isInsured = await _myCareNet.VerifyAssurabilityAsync(request.PatientId, cancellationToken);
        
        // Logique métier encapsulée dans l'entité du Domaine (DDD)
        var invoice = Invoice.CreateNew(request.PatientId, inamiCode, request.BaseAmount, isInsured);
        
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        // Renvoi de l'ID de la facture créée, encapsulé dans un Result de succès
        return Result.Success(invoice.Id);
    }
}
```

### 3.2 Enregistrement au démarrage

```csharp
using Mediarq.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

// Scan des assemblies contenant Commands / Queries / Handlers / Validators / Behaviors de chaque module
builder.Services.AddMediarq(isHttp: true, typeof(GenerateInvoiceCommand).Assembly)
                .AddMediarqRequestLogging()       // LoggingBehavior — trace systématique du flux
                .AddMediarqPerformanceTracking();  // PerformanceBehavior
```

> Pour un démarrage sans réflexion (Native AOT / trimming), préférer `AddMediarqCore(isHttp: true).AddMediarqHandlers()` (registre généré à la compilation) une fois le projet stabilisé.

### 3.3 Endpoint Minimal API — mapping du `Result`

```csharp
using Mediarq.AspNetCore; // ToHttpResult() — mappe Result/ResultError → IResult (+ ProblemDetails RFC 7807)

app.MapPost("/api/billing/invoices", async (GenerateInvoiceCommand command, ISender sender, CancellationToken ct) =>
{
    var result = await sender.Send(command, ct);
    return result.ToHttpResult();
});
```

---

## 4. Intégration du DDD (Domain-Driven Design)

Pour éviter un code "anémique" (où les entités ne sont que des sacs de propriétés `get; set;`), l'état de l'application et les invariants métiers sont défendus au sein du Domaine.

* **Entités & Aggrégats (Aggregate Roots) :** Une entité comme `Invoice` possède des méthodes métiers privatisant ses modificateurs d'état (ex: `invoice.MarkAsPaid(paymentMethod)`).
* **Value Objects :** Des objets immuables sans identité propre (ex: `InamiCode`, `Tariff`). Ils s'auto-valident lors de leur instanciation.
* **Domain Events :** Lorsqu'une action critique se produit (ex: `InvoiceGenerated`), l'entité lève un événement de domaine interne, publié via Mediarq (`INotification` / `IPublisher.Publish`) pour déclencher des effets de bord asynchrones (envoi de l'eAttest vers eHealth, notification par mail au patient). Pour les événements critiques ne tolérant aucune perte, s'appuyer sur `Mediarq.Outbox` (outbox transactionnel EF Core) plutôt qu'une publication in-memory simple.

---

## 5. Gestion des Réseaux Transverses (Mediarq Pipeline Behaviors)

L'un des avantages majeurs de Mediarq est la capacité d'intercepter chaque commande et requête à l'aide de **Pipeline Behaviors** (similaires à des Middlewares), de façon composable et ordonnée (`IOrderBehavior`). Cela permet de centraliser les aspects applicatifs globaux sans polluer les Slices fonctionnelles. Le pipeline est "lean by default" : un behavior inactif (pas de validateur, pas de processor…) n'ajoute ni frame async ni délégué.

```
[ Requête HTTP ]
       │
       ▼
┌────────────────────────────────────────────────────────┐
│ Mediarq Pipeline                                        │
│                                                          │
│  ├── 1. LoggingBehavior (AddMediarqRequestLogging)      │
│  │      Trace systématique du flux                      │
│  ├── 2. ValidationBehavior (intégré, actif si un         │
│  │      IValidator<T> est enregistré — Mediarq.FluentValidation) │
│  ├── 3. TransactionBehavior / Unit of Work               │
│  │      (Mediarq.UnitOfWork + Mediarq.EntityFrameworkCore, │
│  │      requêtes marquées ITransactionalRequest)         │
│  └── 4. AuditBehavior (custom IPipelineBehavior)         │
│                                                          │
│  └───► [ Cas d'utilisation / Feature Handler ]           │
└────────────────────────────────────────────────────────┘
```

* **Logging System :** `AddMediarqRequestLogging()` enregistre automatiquement le nom de la commande, ses paramètres (en masquant les données de santé sensibles) et le temps d'exécution. Combiner avec `Mediarq.Diagnostics`/`Mediarq.OpenTelemetry` pour la traçabilité distribuée.
* **Validation Globale :** Si un `IValidator<T>` (via `AbstractValidator<T>` + `Mediarq.FluentValidation`, ou `Mediarq.DataAnnotations`) existe pour la requête, le `ValidationBehavior` intégré l'exécute automatiquement avant le Handler et court-circuite avec un `Result`/`Result<T>` d'échec (portant une `ValidationError`) en cas d'erreur — pas d'exception levée pour ce cas attendu.
* **Transaction / Unit of Work :** les commandes qui modifient l'état et doivent être atomiques implémentent `ITransactionalRequest` ; `Mediarq.UnitOfWork` + `Mediarq.EntityFrameworkCore` (`EfCoreUnitOfWork<TContext>`) commitent la transaction globale autour du traitement.
* **Audit Trail (Exigence eHealth) :** aucun behavior d'audit n'est fourni nativement — implémenter un `IPipelineBehavior<TRequest, TResponse>` (ou `IPipelineBehavior<TRequest, TResponse>, IOrderBehavior` pour maîtriser l'ordre d'exécution) qui intercepte toutes les commandes modifiant un dossier médical ou une facture, afin d'enregistrer l'historique complet dans la base de logs d'audit (exigence RGPD/eHealth). Exemple de signature Mediarq pour un behavior custom :

```csharp
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    public int Order => 10; // ordre d'exécution explicite (plus petit = plus externe)

    public async Task<TResponse> Handle(
        IMutableRequestContext<TRequest, TResponse> context,
        Func<Task<TResponse>> handle,
        CancellationToken cancellationToken = default)
    {
        // before : capturer l'utilisateur courant (HttpUserContext), la commande, l'horodatage
        var response = await handle();
        // after : persister l'entrée d'audit (succès/échec, diff éventuel)
        return response;
    }
}
```

---

## 6. Migration depuis MediatR (référence historique)

Ce document a été initialement rédigé avec MediatR comme médiateur cible ; le projet utilise désormais **Mediarq**. Les grands repères de correspondance :

| MediatR | Mediarq |
|---|---|
| `IRequest<T>` | `ICommand<T>` / `IQuery<T>` (typage explicite lecture/écriture) |
| `IRequest` (sans retour) | `ICommand` |
| `IRequestHandler<TRequest, TResponse>` | `ICommandHandler<TCommand, TResponse>` / `IQueryHandler<TQuery, TResponse>` |
| `INotification` / `INotificationHandler<T>` | Identique (`INotification` / `INotificationHandler<T>`) |
| `IPipelineBehavior<TRequest, TResponse>` | `IPipelineBehavior<TRequest, TResponse>` (signature différente : `IMutableRequestContext` + délégué `handle`) |
| `services.AddMediatR(...)` | `services.AddMediarq(isHttp:, assemblies)` ou `AddMediarqCore()` + `AddMediarqHandlers()` (AOT) |
| Retour par exception métier | Retour par `Result` / `Result<T>` (railway-oriented) |

Un guide de migration détaillé est disponible dans la documentation officielle de Mediarq ("Migrating from MediatR").
