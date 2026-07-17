# Stratégie de branching — OmniCare

GitFlow allégé, adapté à un développeur principal + assistance IA, et au phasage du
cahier des charges (Phase 1 MVP → Phase 6). Objectif : `main` toujours déployable
(SaaS santé), intégration continue sur `develop`, travail en branches courtes.

## Branches permanentes

| Branche | Rôle | Règles |
|---|---|---|
| `main` | Stable / production. Chaque merge correspond à un jalon déployable. | Jamais de commit direct. Alimentée uniquement par PR depuis `develop` (fin de phase/jalon) ou `hotfix/*`. Taguée à chaque merge (`v0.1.0`, …). |
| `develop` | Intégration de la phase en cours. Doit toujours builder (`dotnet build` + `dotnet test` verts). | Jamais de commit direct (hors ajustements de doc triviaux). Alimentée par PR depuis `feature/*` et `fix/*`. |

## Branches éphémères

| Préfixe | Base | Cible (PR) | Usage / exemple |
|---|---|---|---|
| `feature/<module>-<sujet>` | `develop` | `develop` | Nouvelle slice ou capacité. Ex. `feature/patients-archive-patient`, `feature/agenda-reminders`, `feature/billing-scaffolding` |
| `fix/<sujet>` | `develop` | `develop` | Correction non urgente. Ex. `fix/niss-checksum-2000` |
| `hotfix/<sujet>` | `main` | `main` **puis** report sur `develop` | Bug critique en production uniquement |
| `release/phase-<n>` | `develop` | `main` | Optionnel : stabilisation de fin de phase si `develop` contient déjà du travail de la phase suivante |

Branches supprimées après merge. Une branche = un sujet, durée de vie courte (quelques
jours max) ; découper plutôt que laisser vivre une branche longue.

## Cycle type

```
develop ──┬── feature/patients-xxx ──PR──> develop
          ├── feature/agenda-yyy   ──PR──> develop
          └── fix/zzz              ──PR──> develop
develop ──(fin de jalon, tests verts)──PR──> main ──tag v0.x.0
main    ──── hotfix/aaa ──PR──> main (+ cherry-pick/merge vers develop)
```

## Règles de merge

- **PR obligatoire** vers `main` et `develop` (protection de branche activée).
- **Squash merge** pour `feature/*` et `fix/*` → un commit propre par sujet dans `develop`.
- **Merge commit** (pas de squash) pour `develop` → `main`, afin de préserver l'historique des sujets.
- Une PR n'est mergeable que si `dotnet build` et `dotnet test` passent (CI à brancher —
  en attendant, vérification locale systématique avant merge).
- Pas de force-push sur `main`/`develop`.

## Convention de commits

[Conventional Commits](https://www.conventionalcommits.org/fr/), scope = module :

```
feat(patients): ajout de l'archivage de la fiche patient
fix(agenda): chevauchement non détecté sur créneau adjacent
chore(build): bump EF Core 10.0.x
docs(cahier): précision sur la nomenclature INAMI
refactor(shared-kernel): extraction de UtcTicksConverter
test(patients): couverture des consentements RGPD
```

Types : `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `perf`, `ci`.
Scopes usuels : `patients`, `agenda`, `billing`, `portal`, `ehealth`, `shared-kernel`, `api`, `cahier`, `build`, `ci`.

## Versionnement

SemVer `v0.<jalon>.<patch>` tant que le produit n'est pas en production (Phase 1 MVP
complète ≈ `v0.1.0`, Facturation ≈ `v0.2.0`, …). Passage en `v1.0.0` à la première mise
en production réelle. Tags posés sur `main` uniquement.

## Données de santé — rappel

Jamais de données patient réelles dans le repo : ni dans les commits, ni dans les
fixtures/tests, ni dans les fichiers `*.db` (déjà exclus par `.gitignore`). Les jeux de
test utilisent des identités fictives et des NISS générés (clé mod 97 valide).
