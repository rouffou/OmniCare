[← Sommaire](README.md)

# 1. Présentation du projet

## 1.1 Contexte

OmniCare a vocation à devenir une plateforme SaaS générique destinée à l'ensemble des prestataires de soins de santé conventionnés en Belgique (kinésithérapeutes, médecins généralistes et spécialistes, infirmiers, dentistes, psychologues/psychothérapeutes, etc.).

Le lancement du produit se fait toutefois sur un premier segment ciblé — les kinésithérapeutes indépendants et cabinets de groupe — qui sert de **vertical pilote (Phase 1)**. Ce choix permet de livrer rapidement une offre complète et éprouvée sur un métier précis, tout en validant en conditions réelles un socle fonctionnel et technique conçu dès le départ pour être générique et réutilisable par d'autres professions de santé.

Le périmètre couvert par ce vertical pilote : prise de rendez-vous, dossier médical, facturation INAMI, et à terme téléconsultation et interopérabilité eHealth.

Le projet a démarré par la définition de son architecture technique (Clean Architecture, Vertical Slice, DDD, CQRS/Mediarq sur .NET Core — voir [`architecture_technique_saas.md`](../architecture_technique_saas.md)). Ce cahier des charges définit le périmètre fonctionnel, réglementaire et organisationnel qui doit guider cette implémentation, **en gardant à l'esprit que chaque choix de modélisation (dossier médical, nomenclature d'actes, rôles) doit rester généralisable à d'autres professions de soins.**

## 1.2 Objectifs du projet

- Digitaliser la gestion administrative et clinique d'un cabinet de kinésithérapie (Phase 1), sur un socle réutilisable pour d'autres professions de santé.
- Automatiser et sécuriser la facturation INAMI (tiers payant, télétransmission MyCareNet), en tenant compte du fait que la nomenclature de codes diffère selon la profession conventionnée.
- Réduire la charge administrative des praticiens (agenda, rappels, facturation), quelle que soit leur profession à terme.
- Offrir au patient un point d'accès autonome (prise de RDV, documents, téléconsultation) commun à tous les prestataires qu'il consulte sur la plateforme.
- Garantir la conformité RGPD et aux exigences eHealth belges dès la conception (privacy by design), ces exigences étant transverses à toutes les professions de santé.
- Construire une base technique **modulaire, générique et évolutive**, permettant d'une part l'ajout progressif de nouveaux modules métier, d'autre part l'onboarding de nouvelles professions de santé sans refonte du socle (voir [4.6 Extensibilité multi-professions](04-6-extensibilite-multi-professions.md)).

## 1.3 Enjeux

- **Réglementaire** : données de santé (catégorie sensible RGPD), obligations INAMI/eHealth, secret médical — communes à toutes les professions visées.
- **Métier** : adoption par des praticiens peu technophiles, besoin de simplicité d'usage ; hétérogénéité des pratiques et de la nomenclature selon la profession.
- **Technique** : fiabilité de la facturation (impact financier direct), disponibilité de l'agenda (usage quotidien), interopérabilité avec les plateformes eHealth belges.
- **Produit/Stratégique** : éviter que les choix de conception "Phase 1" (spécifiques à la kinésithérapie) ne deviennent un frein à l'ouverture à d'autres professions — nécessité d'une modélisation générique dès le départ (ex. "profession du prestataire" comme paramètre, et non comme hypothèse implicite du code).

---
[← Sommaire](README.md) · [Suivant : Parties prenantes →](02-parties-prenantes.md)
