[← Sommaire](README.md)

# 6. Architecture technique (résumé)

Le socle technique est déjà spécifié dans [`architecture_technique_saas.md`](../architecture_technique_saas.md) et sert de référence contraignante pour l'implémentation :

- **Backend** : .NET Core, Clean Architecture + Vertical Slice Architecture.
- **Pattern** : CQRS via [Mediarq](https://www.nuget.org/packages/Mediarq/) (séparation Commands/Queries, et non MediatR — librairie sans dépendance, `Result<T>` railway-oriented, pipeline lean-by-default).
- **Domain-Driven Design** : entités riches, Value Objects (ex. `InamiCode`, `Amount`), Domain Events (`INotification` Mediarq).
- **Modules** identifiés à ce stade : `Patients`, `Agenda`, `Billing` — à compléter par `Portal`/`Teleconsultation` et `EHealth` selon le périmètre fonctionnel décrit en section 4.
- **Pipeline Behaviors** : Logging (`AddMediarqRequestLogging`), Validation (FluentValidation via `Mediarq.FluentValidation`, intégré et actif seulement si un validateur existe), Transaction/Unit of Work (`Mediarq.UnitOfWork` + `Mediarq.EntityFrameworkCore`), Audit Trail (behavior custom).
- **Intégrations externes** : MyCareNet (assurabilité, télétransmission, avec retry/circuit breaker via `Mediarq.Polly`), extensible à eHealth.

Ce cahier des charges fonctionnel doit se traduire par la création de nouveaux modules suivant cette même structure (`Domain/`, `Features/`, `Infrastructure/`) pour chaque domaine métier listé en section 4.

**Contrainte de conception transverse (vision multi-professions — voir [4.6](04-6-extensibilite-multi-professions.md))** : le vocabulaire du Domaine doit rester neutre vis-à-vis de la kinésithérapie (`Practitioner` plutôt que `Kinesitherapist`, `ClinicalRecord` plutôt que `KineBilan`, `ActCode` générique dérivant `InamiCode`). La nomenclature d'actes et les champs du dossier médical doivent être pilotés par un référentiel configurable par profession, et non codés en dur. Cette contrainte doit être validée avec l'équipe technique avant le démarrage du développement des modules Domain.

---
[← Exigences non fonctionnelles](05-exigences-non-fonctionnelles.md) · [Sommaire](README.md) · [Suivant : Rôles et profils utilisateurs →](07-roles-utilisateurs.md)
