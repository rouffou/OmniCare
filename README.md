# OmniCare

[![CI](https://github.com/rouffou/OmniCare/actions/workflows/ci.yml/badge.svg?branch=develop)](https://github.com/rouffou/OmniCare/actions/workflows/ci.yml)

Plateforme SaaS de gestion pour prestataires de soins de santé belges (INAMI / MyCareNet / eHealth).
Phase 1 : cabinets de kinésithérapie, sur un socle générique multi-professions.

## Stack

.NET 10 · Minimal API · Clean Architecture + Vertical Slice + DDD · CQRS via [Mediarq](https://github.com/rouffou/mediarq) · EF Core (SQLite en dev)

## Démarrage rapide

```bash
dotnet tool restore          # dotnet-ef (migrations)
dotnet build OmniCare.slnx
dotnet test OmniCare.slnx
dotnet run --project src/Api # http://localhost:5210 — migrations appliquées au boot en dev
```

## Structure

| Dossier | Contenu |
|---|---|
| `src/BuildingBlocks/SharedKernel/` | Primitives DDD, value objects transverses (NISS, `InamiCode`, `HealthProfession`), audit trail |
| `src/Modules/{Patients,Agenda,Billing}/` | Modules métier (Vertical Slice : `Domain/`, `Features/`, `Infrastructure/`) |
| `src/Api/` | Hôte Minimal API (pipeline Mediarq : logging → validation → audit → unit of work) |
| `tests/` | Tests unitaires du Domain (xUnit) |

## Contribuer

Stratégie de branching, conventions de commit et règles de merge : [docs/BRANCHING.md](docs/BRANCHING.md).
Guidance agent/IA : [CLAUDE.md](CLAUDE.md).

⚠️ Jamais de données patient réelles dans le repo (code, tests, fixtures, bases locales).
