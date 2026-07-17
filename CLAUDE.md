# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## État du projet

Ce repo en est au stade "cahier des charges" : aucun code n'a encore été écrit. Les seuls fichiers présents sont de la documentation de conception dans `cahier_des_charges/`. Ce fichier sert de point de départ pour démarrer l'implémentation du backend à partir de cette documentation.

Avant de générer du code, lire dans l'ordre :
1. `cahier_des_charges/architecture_technique_saas.md` — architecture technique de référence (Clean Architecture, Vertical Slice, DDD, CQRS/Mediarq), avec un exemple de code complet (commande `GenerateInvoice`).
2. `cahier_des_charges/omnicare/README.md` — cahier des charges fonctionnel complet (sommaire + liens vers chaque section).

## Vision produit (résumé)

OmniCare est une plateforme SaaS de gestion pour prestataires de soins de santé belges, avec intégration INAMI / MyCareNet / eHealth. La Phase 1 cible un seul vertical métier — les cabinets de kinésithérapie — mais le socle doit rester générique pour accueillir d'autres professions (médecins, infirmiers, dentistes, psychologues) sans refonte majeure (détails : `cahier_des_charges/omnicare/04-6-extensibilite-multi-professions.md`).

**Contrainte de nommage transverse, à respecter dans tout le code écrit** : ne jamais coder en dur du vocabulaire spécifique à la kinésithérapie dans le Domain. Utiliser des noms génériques :
- `Practitioner` (pas `Kinesitherapist`)
- `ClinicalRecord` (pas `KineBilan`)
- `ActCode` générique dérivant `InamiCode`, piloté par un référentiel configurable par profession

## Architecture cible

Stack : .NET Core (backend), API HTTP en Minimal API.

Trois paradigmes combinés (voir `architecture_technique_saas.md` pour le détail) :
- **Vertical Slice Architecture** : le code est découpé par cas d'utilisation métier (`Features/`), pas par couches techniques horizontales.
- **Clean Architecture + DDD** : le Domain est isolé de l'infrastructure (bases de données, APIs eHealth/MyCareNet). Entités riches avec méthodes métier (pas de `get;set;` anémiques), Value Objects auto-validants, Domain Events.
- **CQRS via Mediarq** : séparation stricte Commands (écriture) / Queries (lecture), chaque Command/Query ayant son propre Handler. Les handlers renvoient un `Result<T>` (railway-oriented) plutôt que de lever des exceptions pour les échecs métier attendus.

Structure de module type, à répliquer pour chaque nouveau module métier :
```
src/Modules/<Module>/
├── Domain/                  Entités, Value Objects, Domain Events, Exceptions
├── Features/<UseCase>/      Command|Query (ICommand<Result<T>>) + Validator (FluentValidation via Mediarq.FluentValidation) + Handler (ICommandHandler, Mediarq) + Endpoint (Minimal API)
└── Infrastructure/          Persistence (EF Core), Services (clients d'API externes type MyCareNet)
```

**Médiateur : [Mediarq](https://www.nuget.org/packages/Mediarq/)** (et non MediatR). C'est une librairie CQRS légère, sans dépendance, avec `Result`/`Result<T>` (railway-oriented), notifications (`INotification`), pipeline de behaviors composable et lean-by-default, et dispatch sans réflexion via source generator (Native AOT-friendly).

Packages à installer selon le besoin (ne pas tout référencer d'un coup) :
- `Mediarq` — meta-package : `Mediarq.Core` + extensions légères (ASP.NET Core, FluentValidation, DataAnnotations, Caching, Diagnostics, UnitOfWork). Référence par défaut.
- `Mediarq.EntityFrameworkCore` — `EfCoreUnitOfWork<TContext>`, nécessaire dès qu'un module persiste via EF Core.
- `Mediarq.Diagnostics` / `Mediarq.OpenTelemetry` — observabilité (Activity, métriques).
- `Mediarq.Idempotency` — sécuriser les commandes sensibles (facturation, télétransmission) contre les doublons.
- `Mediarq.Outbox` — outbox transactionnel EF Core pour les Domain Events critiques (ex. `InvoiceGenerated`).
- `Mediarq.Polly` — retry/timeout/circuit breaker pour les appels MyCareNet/eHealth.
- `Mediarq.Templates` — `dotnet new mediarq-feature -n <UseCase>` pour scaffolder une slice (Command + Handler + Validator).

Détail complet des packages et exemple de code (commande `GenerateInvoice`, endpoint, behaviors) : `cahier_des_charges/architecture_technique_saas.md`.

Pipeline Behaviors Mediarq à mettre en place globalement dès les premiers modules (interceptent chaque Command/Query) :
- **Logging** : `AddMediarqRequestLogging()` — trace systématique du flux, en masquant les données de santé sensibles.
- **Validation** : `ValidationBehavior` intégré, actif automatiquement dès qu'un `IValidator<T>` (FluentValidation via `Mediarq.FluentValidation`) est enregistré pour la commande/requête — court-circuite avec un `Result` d'échec avant d'atteindre le Handler.
- **Transaction/Unit of Work** : requêtes marquées `ITransactionalRequest`, commit géré par `Mediarq.UnitOfWork` + `Mediarq.EntityFrameworkCore`.
- **Audit Trail** : pas de behavior natif — implémenter un `IPipelineBehavior<TRequest, TResponse>` custom (+ `IOrderBehavior` pour l'ordonnancement). Toute commande modifiant un dossier médical ou une facture doit être tracée dans une base de logs d'audit (exigence RGPD/eHealth).

## Modules fonctionnels à scaffolder

Ordre de priorité issu du phasage (`cahier_des_charges/omnicare/08-phasage.md`) :

1. `Patients` — dossiers médicaux, fiches patients (`cahier_des_charges/omnicare/04-1-patients-dossiers-medicaux.md`)
2. `Agenda` — rendez-vous (`cahier_des_charges/omnicare/04-2-agenda-rendez-vous.md`)
3. `Billing` — facturation INAMI/MyCareNet (`cahier_des_charges/omnicare/04-3-facturation-inami-mycarenet.md`)
4. `Portal` / `Teleconsultation` — portail patient (`cahier_des_charges/omnicare/04-4-portail-patient-teleconsultation.md`)
5. `EHealth` — intégration étendue, phase ultérieure (`cahier_des_charges/omnicare/04-5-integration-ehealth.md`)

## Commandes de développement

La solution est `OmniCare.slnx` (format slnx, SDK .NET 10) :
- `dotnet build OmniCare.slnx` — build de la solution
- `dotnet test OmniCare.slnx` — exécution des tests (projet `tests/OmniCare.UnitTests`, xUnit)
- `dotnet test --filter FullyQualifiedName~<NomDuTest>` — exécuter un test unique
- `dotnet run --project src/Api` — lancer l'API localement (http://localhost:5210, une base SQLite par module : `omnicare-patients.db`, `omnicare-agenda.db`, `omnicare-audit.db`, schéma créé via `EnsureCreated` en dev — à remplacer par des migrations EF Core avant tout déploiement)
- `dotnet format OmniCare.slnx` — formatage (règles dans `.editorconfig`)

Structure réelle :
- `src/BuildingBlocks/SharedKernel/` — primitives DDD (`Entity`, `AggregateRoot`, `IDomainEvent`), Value Objects transverses (`NationalRegistryNumber`, `ActCode`/`InamiCode`, `HealthProfession`), `AuditBehavior` + abstractions (`ICurrentUserService`, `IAuditTrailStore`), `BusinessFailures` (point unique de construction des `Result` d'échec), `ModuleDbContext` (dispatch des Domain Events après SaveChanges).
- `src/Modules/Patients/` et `src/Modules/Agenda/` — modules Vertical Slice (`Domain/`, `Features/<UseCase>/`, `Infrastructure/Persistence/`), chacun avec son composition root (`PatientsModule.cs`, `AgendaModule.cs` : `Add<Module>Module()` + `Map<Module>Module()`).
- `src/Api/` — hôte Minimal API : DI Mediarq + validateurs, `AuditDbContext` (journal d'audit séparé des contextes métier), `ModularUnitOfWork` (IUnitOfWork composite), `Program.cs`.
- **Transactions** : les commandes d'écriture sont marquées `ITransactionalRequest` (`Mediarq.UnitOfWork`) — le `UnitOfWorkBehavior` committe après le handler, uniquement si le `Result` est un succès. Les handlers ne doivent PAS appeler `SaveChangesAsync` eux-mêmes.
- Versions de packages centralisées dans `Directory.Packages.props` (Central Package Management).

Pièges vérifiés à l'implémentation (ne pas re-découvrir) :
- **Namespaces Mediarq réels** : pas de `using Mediarq;` — les types vivent dans `Mediarq.Core.Common.Requests.Command|Query|Notifications|Abstraction`, `Mediarq.Core.Common.Results`, `Mediarq.Core.Common.Pipeline`, `Mediarq.Core.Common.Contexts`, `Mediarq.Core.Mediators` (ISender/IPublisher), `Mediarq.Extensions` (AddMediarq), `Mediarq.AspNetCore` (ToHttpResult), `Mediarq.FluentValidation`.
- **Ordre d'enregistrement** : `AddValidatorsFromAssemblies(...)` + `AddMediarqFluentValidation()` doivent précéder `AddMediarq(...)` (scan), sinon la validation ne fait silencieusement rien (documenté dans le README du package).
- **Échecs métier** : construire les `Result` d'échec uniquement via `OmniCare.SharedKernel.Application.BusinessFailures` ; `ErrorType.Failure` est mappé HTTP 500 par `ToHttpResult`, d'où `ErrorType.Validation` (400) pour les règles métier et `Conflict`/`NotFound` sinon.
- **EF Core + Id client (Guid v7 au constructeur)** : appeler `UseClientGeneratedIds(modelBuilder)` (base `ModuleDbContext`) en fin de `OnModelCreating`, sinon une entité enfant ajoutée à un agrégat déjà tracké part en UPDATE au lieu d'INSERT (`DbUpdateConcurrencyException`).
- **SQLite + DateTimeOffset** : les comparaisons/tris SQL sur `DateTimeOffset` ne sont pas traduits par le provider SQLite — utiliser `UtcTicksConverter` (SharedKernel) pour toute colonne filtrée/triée (ex. `SlotStart`/`SlotEnd`).
- **Value Objects persistés** : propriétés en `{ get; private set; }` (pas `{ get; }` seul), sinon EF ne peut pas lier le constructeur des types owned.
- **UnitOfWork multi-contextes** : `UnitOfWorkBehavior` (Mediarq.UnitOfWork) n'injecte qu'un seul `IUnitOfWork` — ne PAS appeler `AddMediarqEntityFrameworkCore<T>()` pour chaque DbContext (le dernier écraserait les autres). Utiliser le composite `ModularUnitOfWork` (src/Api/Infrastructure) et y ajouter tout nouveau DbContext de module. SQLite multi-fichiers = pas d'atomicité inter-modules ; pour un événement critique inter-modules, passer par `Mediarq.Outbox` (prévu Phase 2 Billing, non câblé).
- **Doc Mediarq de référence** : le repo local `C:\Users\nicol\source\repos\Mediarq` (projet perso de l'auteur du repo) — `docs/guides/wiring-extensions.md` (ordre d'enregistrement complet) et `docs/guides/troubleshooting.md`. Les exemples `using Mediarq;` du cahier des charges ne compilent pas (aucun namespace racine `Mediarq` dans la lib).

## Git & branching

Remote : https://github.com/rouffou/OmniCare. Stratégie complète : [docs/BRANCHING.md](docs/BRANCHING.md).
Résumé : GitFlow allégé — `main` (stable, taguée, PR uniquement), `develop` (intégration, PR uniquement),
branches courtes `feature/<module>-<sujet>` / `fix/<sujet>` basées sur `develop`, `hotfix/*` basées sur `main`.
Squash merge vers `develop`, merge commit `develop` → `main`. Commits en
[Conventional Commits](https://www.conventionalcommits.org/fr/) avec scope = module (`feat(patients): …`).
Toujours créer une branche depuis `develop` avant de coder ; jamais de commit direct sur `main`/`develop`.

## Contraintes réglementaires à garder en tête en permanence

- Données de santé = catégorie sensible RGPD (art. 9) → chiffrement, contrôle d'accès par rôle, audit trail systématique sur le Domain.
- Toute facturation passe par une vérification d'assurabilité MyCareNet et repose sur la nomenclature INAMI.
- Secret médical : séparer strictement les données administratives (accessibles au secrétariat) des données cliniques (réservées au praticien).

Détails complets : `cahier_des_charges/omnicare/05-exigences-non-fonctionnelles.md`.
