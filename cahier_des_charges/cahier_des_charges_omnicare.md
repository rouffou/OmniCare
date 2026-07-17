# Cahier des Charges — OmniCare
## Plateforme SaaS de gestion pour prestataires de soins de santé
### Phase 1 : cabinets de Kinésithérapie — conçue pour une extension multi-professions

**Version :** 1.1
**Date :** 17 juillet 2026
**Statut :** Draft de travail

---

## 1. Présentation du projet

### 1.1 Contexte

OmniCare a vocation à devenir une plateforme SaaS générique destinée à l'ensemble des prestataires de soins de santé conventionnés en Belgique (kinésithérapeutes, médecins généralistes et spécialistes, infirmiers, dentistes, psychologues/psychothérapeutes, etc.).

Le lancement du produit se fait toutefois sur un premier segment ciblé — les kinésithérapeutes indépendants et cabinets de groupe — qui sert de **vertical pilote (Phase 1)**. Ce choix permet de livrer rapidement une offre complète et éprouvée sur un métier précis, tout en validant en conditions réelles un socle fonctionnel et technique conçu dès le départ pour être générique et réutilisable par d'autres professions de santé.

Le périmètre couvert par ce vertical pilote : prise de rendez-vous, dossier médical, facturation INAMI, et à terme téléconsultation et interopérabilité eHealth.

Le projet a démarré par la définition de son architecture technique (Clean Architecture, Vertical Slice, DDD, CQRS/Mediarq sur .NET Core — voir `architecture_technique_saas.md`). Ce cahier des charges définit le périmètre fonctionnel, réglementaire et organisationnel qui doit guider cette implémentation, **en gardant à l'esprit que chaque choix de modélisation (dossier médical, nomenclature d'actes, rôles) doit rester généralisable à d'autres professions de soins.**

### 1.2 Objectifs du projet

- Digitaliser la gestion administrative et clinique d'un cabinet de kinésithérapie (Phase 1), sur un socle réutilisable pour d'autres professions de santé.
- Automatiser et sécuriser la facturation INAMI (tiers payant, télétransmission MyCareNet), en tenant compte du fait que la nomenclature de codes diffère selon la profession conventionnée.
- Réduire la charge administrative des praticiens (agenda, rappels, facturation), quelle que soit leur profession à terme.
- Offrir au patient un point d'accès autonome (prise de RDV, documents, téléconsultation) commun à tous les prestataires qu'il consulte sur la plateforme.
- Garantir la conformité RGPD et aux exigences eHealth belges dès la conception (privacy by design), ces exigences étant transverses à toutes les professions de santé.
- Construire une base technique **modulaire, générique et évolutive**, permettant d'une part l'ajout progressif de nouveaux modules métier, d'autre part l'onboarding de nouvelles professions de santé sans refonte du socle (voir section 4.6).

### 1.3 Enjeux

- **Réglementaire** : données de santé (catégorie sensible RGPD), obligations INAMI/eHealth, secret médical — communes à toutes les professions visées.
- **Métier** : adoption par des praticiens peu technophiles, besoin de simplicité d'usage ; hétérogénéité des pratiques et de la nomenclature selon la profession.
- **Technique** : fiabilité de la facturation (impact financier direct), disponibilité de l'agenda (usage quotidien), interopérabilité avec les plateformes eHealth belges.
- **Produit/Stratégique** : éviter que les choix de conception "Phase 1" (spécifiques à la kinésithérapie) ne deviennent un frein à l'ouverture à d'autres professions — nécessité d'une modélisation générique dès le départ (ex. "profession du prestataire" comme paramètre, et non comme hypothèse implicite du code).

---

## 2. Parties prenantes

| Rôle | Description |
|---|---|
| Praticien / Prestataire de soins | Utilisateur principal : gère patients, agenda, facturation. Kinésithérapeute en Phase 1 ; profil conçu pour accueillir d'autres professions par la suite (médecin généraliste/spécialiste, infirmier, dentiste, psychologue…) |
| Secrétariat / assistant(e) | Gestion agenda et accueil, accès restreint au dossier médical |
| Patient | Utilisateur du portail : RDV, documents, téléconsultation — potentiellement suivi par plusieurs prestataires de professions différentes sur la plateforme |
| Administrateur cabinet | Gestion des utilisateurs, des accès et de la configuration du cabinet |
| INAMI / MyCareNet / eHealth | Organismes tiers — flux de télétransmission et de vérification d'assurabilité, communs à toutes les professions conventionnées |
| Éditeur / équipe projet | Conception, développement, exploitation de la plateforme |

---

## 3. Périmètre du projet

### 3.1 Dans le périmètre (in scope)

1. Gestion des patients & dossiers médicaux
2. Agenda & gestion des rendez-vous
3. Facturation & télétransmission INAMI/MyCareNet
4. Portail patient & téléconsultation
5. Intégration eHealth (posée en phase ultérieure, mais anticipée dès l'architecture)
6. Extensibilité multi-professions de santé (généricité du socle dès la conception, activation progressive — voir 4.6)

### 3.2 Hors périmètre (out of scope, à ce stade)

- Comptabilité générale du cabinet (au-delà de la facturation patient) — intégration possible avec un logiciel comptable tiers, à définir ultérieurement.
- Gestion des stocks / matériel médical.
- Module de télé-expertise entre praticiens.
- Application mobile native (le portail patient est envisagé en web responsive dans un premier temps).

Ces exclusions pourront être revues dans une phase ultérieure du projet.

---

## 4. Spécifications fonctionnelles

### 4.1 Module Patients & Dossiers médicaux

**Objectif** : centraliser l'identité administrative et le suivi clinique du patient.

Fonctionnalités attendues :
- Fiche d'identité patient (coordonnées, mutuelle, médecin traitant, personne de contact).
- Numéro de registre national et données d'assurabilité (vérifiées via MyCareNet).
- Dossier médical structuré : anamnèse, bilans cliniques (spécifiques à la profession du praticien — bilan kinésithérapique en Phase 1), plans de traitement, comptes-rendus de séance.
- Historique complet et chronologique des séances et documents associés.
- Gestion des documents joints (prescriptions médicales, comptes-rendus, imagerie).
- Consentements RGPD et traçabilité des accès au dossier (qui a consulté quoi, et quand).
- Recherche et filtrage des patients (nom, statut, pathologie, praticien référent).
- Gestion des prescriptions médicales (nombre de séances prescrites, suivi de la consommation).

Exigences particulières :
- Le dossier médical est un sous-ensemble protégé nécessitant une traçabilité d'accès renforcée (audit trail — cf. architecture technique, section Pipeline Behaviors).
- Séparation claire entre données administratives (accessibles au secrétariat) et données cliniques (réservées au praticien).

### 4.2 Module Agenda & Rendez-vous

**Objectif** : optimiser la gestion du temps du praticien et réduire les rendez-vous manqués.

Fonctionnalités attendues :
- Agenda multi-praticien (vue cabinet) et agenda individuel.
- Création, modification, annulation de rendez-vous, avec gestion des récurrences (séries de séances).
- Gestion des créneaux disponibles, des durées de séance configurables par type d'acte.
- Rappels automatiques (SMS/email) au patient avant le rendez-vous.
- Gestion des salles / équipements si le cabinet en dispose.
- Liste d'attente et gestion des désistements.
- Statuts de rendez-vous (planifié, confirmé, réalisé, annulé, no-show).
- Synchronisation possible avec un calendrier externe (Google Calendar/Outlook) — à évaluer en phase ultérieure.

### 4.3 Module Facturation & INAMI/MyCareNet

**Objectif** : sécuriser et automatiser le cycle de facturation belge basé sur la nomenclature INAMI (kinésithérapie en Phase 1, structure conçue pour accueillir la nomenclature d'autres professions — voir 4.6).

Fonctionnalités attendues :
- Génération de factures basées sur les codes de nomenclature INAMI (voir `InamiCode` dans l'architecture technique).
- Vérification d'assurabilité du patient via MyCareNet avant facturation.
- Gestion du tiers payant et du régime BIM/OMNIO.
- Télétransmission des attestations de soins (eAttest) vers les organismes assureurs via eHealth/MyCareNet.
- Suivi des statuts de télétransmission (envoyée, acceptée, rejetée) et gestion des rejets/corrections.
- Facturation patient (part non remboursée) avec génération de documents PDF conformes.
- Suivi des paiements, relances, et état des comptes patients.
- Journalisation immuable de toute émission ou modification de facture (exigence d'audit).
- Export comptable (format à définir avec l'expert-comptable du cabinet).

### 4.4 Module Portail Patient & Téléconsultation

**Objectif** : offrir au patient un accès autonome à ses informations et à la prise de rendez-vous.

Fonctionnalités attendues :
- Authentification sécurisée du patient (email/mot de passe, option eID/itsme à évaluer).
- Prise de rendez-vous en ligne selon les disponibilités du praticien.
- Consultation des documents partagés (comptes-rendus, prescriptions, factures).
- Rappels et notifications (RDV à venir, documents disponibles).
- Téléconsultation : visioconférence sécurisée intégrée pour les séances à distance, lorsque cliniquement pertinent.
- Questionnaires pré-consultation (anamnèse en ligne) — optionnel selon phase.
- Gestion du consentement RGPD directement depuis le portail.

### 4.5 Intégration eHealth (phase ultérieure)

**Objectif** : anticiper l'interopérabilité complète avec l'écosystème eHealth belge, au-delà de MyCareNet.

Éléments à intégrer progressivement :
- Authentification forte via eHealth (certificats, eID).
- Connexion au Réseau Santé Wallon / Vitalink / Intermed (selon la région) pour le partage de données patient inter-praticiens.
- Sumehr (Résumé électronique du dossier médical) si pertinent pour la coordination des soins.
- Consentement patient centralisé (plateforme de consentement eHealth).
- Veille réglementaire continue, les exigences eHealth évoluant régulièrement.

Cette intégration est posée comme une extension du module Facturation/MyCareNet déjà prévu dans l'architecture technique (`IMyCareNetService`), afin de limiter la dette technique lors de son implémentation future.

### 4.6 Extensibilité multi-professions de santé (vision cible, activation progressive)

**Objectif** : garantir que le socle conçu en Phase 1 pour la kinésithérapie puisse accueillir d'autres professions de santé sans refonte majeure.

**Professions cibles identifiées à ce stade** (par ordre indicatif, à prioriser ultérieurement) :
- Médecins généralistes et spécialistes
- Infirmiers / infirmières
- Dentistes
- Psychologues / psychothérapeutes

D'autres professions conventionnées pourront être ajoutées par la suite selon la stratégie produit.

**Implications fonctionnelles à anticiper dès la Phase 1** :
- **Dossier médical** : la structure du dossier (anamnèse, bilans, comptes-rendus) doit être conçue comme un ensemble de sections/champs configurables par profession, plutôt que des champs figés "kiné" (ex. remplacer "bilan kinésithérapique" par un concept générique de "bilan clinique" dont le contenu est paramétrable).
- **Nomenclature de facturation** : chaque profession dispose de ses propres codes de nomenclature INAMI. Le Value Object `InamiCode` et la logique de calcul des honoraires doivent être conçus comme génériques, avec un référentiel de codes chargé par profession (pas de logique métier codée en dur pour la kinésithérapie uniquement).
- **Types d'actes et durées de séance** : à définir par profession (ex. consultation de 15 min chez un généraliste vs séance de 30 min chez un kiné).
- **Profil praticien** : ajout d'un attribut "profession de santé" sur le prestataire, déterminant les champs, la nomenclature et les règles métier applicables.
- **Portail patient** : un même patient doit pouvoir être suivi par plusieurs prestataires de professions différentes sur la plateforme, avec une vue consolidée si pertinent.
- **Réglementaire** : certaines professions (médecins, dentistes) ont des exigences eHealth/INAMI ou des obligations de tenue de dossier différentes (ex. Sumehr pour les médecins) — à cartographier profession par profession avant ouverture.

**Ce que cela implique pour l'architecture technique** (à formaliser avec l'équipe technique) :
- Éviter tout couplage fort entre les modules `Domain`/`Features` et le vocabulaire spécifique à la kinésithérapie ; privilégier une terminologie neutre (`Practitioner`, `ClinicalRecord`, `ActCode`) plutôt que des noms trop spécifiques (`Kinesitherapist`, `KineBilan`).
- Prévoir un mécanisme de configuration/paramétrage par profession (référentiels d'actes, champs de dossier, règles de facturation) plutôt que des branches de code par métier.

Cette section pose la vision cible ; elle ne constitue pas un engagement de livraison en Phase 1, mais une contrainte de conception à respecter dès maintenant pour ne pas hypothéquer l'évolution future du produit.

---

## 5. Exigences non fonctionnelles

### 5.1 Sécurité & confidentialité
- Chiffrement des données de santé au repos et en transit (TLS, chiffrement base de données).
- Authentification forte (MFA) pour les praticiens et le personnel administratif.
- Gestion fine des droits d'accès par rôle (praticien, secrétariat, administrateur, patient).
- Audit trail systématique sur toute action touchant un dossier médical ou une facture (déjà prévu au niveau architecture via les Pipeline Behaviors Mediarq).

### 5.2 Conformité réglementaire
- Conformité RGPD (données de santé = catégorie particulière, art. 9).
- Respect du secret médical et des recommandations de l'INAMI.
- Hébergement des données de santé chez un hébergeur certifié / conforme à la réglementation belge en vigueur.
- Politique de conservation et de purge des données conforme aux durées légales de conservation des dossiers médicaux.

### 5.3 Disponibilité & performance
- Disponibilité cible du service : à définir (SLA), avec attention particulière à l'agenda et à la facturation (usage quotidien critique).
- Temps de réponse acceptable sur les opérations courantes (consultation agenda, ouverture dossier patient).
- Plan de sauvegarde et de reprise après sinistre (RPO/RTO à définir).

### 5.4 Scalabilité & évolutivité
- Architecture modulaire (Vertical Slice) permettant l'ajout de nouveaux modules métier sans régression sur l'existant.
- Capacité à supporter la montée en charge multi-cabinets (multi-tenant à confirmer).
- **Généricité du modèle métier** : dossier médical, nomenclature d'actes et rôles praticien conçus dès la Phase 1 comme paramétrables par profession de santé, afin de permettre l'onboarding d'autres professions sans refonte du socle (voir section 4.6).

### 5.5 Utilisabilité
- Interface simple et accessible pour des praticiens non technophiles.
- Portail patient accessible et responsive (mobile-first recommandé).
- Support multilingue à minima FR/NL (contexte belge), EN en option.

---

## 6. Architecture technique (résumé)

Le socle technique est déjà spécifié dans `architecture_technique_saas.md` et sert de référence contraignante pour l'implémentation :

- **Backend** : .NET Core, Clean Architecture + Vertical Slice Architecture.
- **Pattern** : CQRS via [Mediarq](https://www.nuget.org/packages/Mediarq/) (séparation Commands/Queries, et non MediatR).
- **Domain-Driven Design** : entités riches, Value Objects (ex. `InamiCode`, `Amount`), Domain Events.
- **Modules** identifiés à ce stade : `Patients`, `Agenda`, `Billing` — à compléter par `Portal`/`Teleconsultation` et `EHealth` selon le périmètre fonctionnel ci-dessus.
- **Pipeline Behaviors** : Logging, Validation (FluentValidation), Transaction/Unit of Work, Audit Trail.
- **Intégrations externes** : MyCareNet (assurabilité, télétransmission), extensible à eHealth.

Ce cahier des charges fonctionnel doit se traduire par la création de nouveaux modules suivant cette même structure (`Domain/`, `Features/`, `Infrastructure/`) pour chaque domaine métier listé en section 4.

**Contrainte de conception transverse (vision multi-professions — voir 4.6)** : le vocabulaire du Domaine doit rester neutre vis-à-vis de la kinésithérapie (`Practitioner` plutôt que `Kinesitherapist`, `ClinicalRecord` plutôt que `KineBilan`, `ActCode` générique dérivant `InamiCode`). La nomenclature d'actes et les champs du dossier médical doivent être pilotés par un référentiel configurable par profession, et non codés en dur. Cette contrainte doit être validée avec l'équipe technique avant le démarrage du développement des modules Domain.

---

## 7. Rôles et profils utilisateurs

| Profil | Accès dossier médical | Accès facturation | Accès agenda | Administration |
|---|---|---|---|---|
| Praticien (kinésithérapeute en Phase 1) | Lecture/écriture complète | Lecture/écriture | Lecture/écriture | Non |
| Secrétariat | Lecture partielle (administratif) | Lecture/écriture | Lecture/écriture | Non |
| Administrateur cabinet | Selon habilitation | Lecture | Lecture | Oui |
| Patient (portail) | Lecture de son propre dossier | Lecture de ses factures | Prise de RDV | Non |

*(Matrice à affiner en phase de conception détaillée.)*

---

## 8. Phasage proposé

| Phase | Contenu | Objectif |
|---|---|---|
| Phase 0 | Architecture technique (fait) | Socle technique validé |
| Phase 1 (MVP) | Patients & Dossiers médicaux + Agenda | Digitaliser la gestion de base du cabinet |
| Phase 2 | Facturation & INAMI/MyCareNet | Sécuriser le cycle de facturation |
| Phase 3 | Portail Patient (RDV en ligne, documents) | Autonomiser le patient |
| Phase 4 | Téléconsultation | Étendre l'offre de soins à distance |
| Phase 5 | Intégration eHealth étendue | Interopérabilité avec l'écosystème santé belge |
| Phase 6 | Ouverture multi-professions (généricisation du référentiel d'actes et du dossier médical, onboarding d'une 2e profession pilote) | Élargir le marché adressable au-delà de la kinésithérapie |

Ce phasage est une proposition de séquencement à valider ; il pourra être ajusté selon les priorités métier. La Phase 6 dépend de la bonne application, dès les Phases 1 à 5, des contraintes de généricité décrites en section 4.6 — un socle trop spécifique à la kinésithérapie augmenterait significativement le coût de cette ouverture.

---

## 9. Livrables attendus

- Spécifications fonctionnelles détaillées par module (issues de ce cahier des charges).
- Maquettes / wireframes des interfaces praticien et patient.
- Backend modulaire conforme à l'architecture technique de référence.
- Documentation d'intégration MyCareNet/eHealth.
- Plan de tests et jeu de recette par module.
- Documentation utilisateur (praticien et patient).

---

## 10. Critères d'acceptation (à affiner)

- Chaque module fonctionnel est livré avec ses tests unitaires et d'intégration.
- La facturation INAMI est validée par un test de télétransmission réel (ou environnement de test MyCareNet) avant mise en production.
- L'audit trail est vérifiable sur toute action de modification d'un dossier médical ou d'une facture.
- Le parcours patient (prise de RDV → consultation → facturation → paiement) est testé de bout en bout.

---

## 11. Glossaire

| Terme | Définition |
|---|---|
| INAMI | Institut National d'Assurance Maladie-Invalidité (Belgique) |
| MyCareNet | Plateforme d'échange sécurisé entre prestataires de soins et organismes assureurs belges |
| eHealth | Plateforme fédérale belge d'échange de données de santé |
| eAttest | Attestation de soins électronique |
| BIM/OMNIO | Statuts belges ouvrant droit à une intervention majorée (tiers payant social) |
| Sumehr | Résumé électronique du dossier médical (Summarized Electronic Health Record) |
| RGPD | Règlement Général sur la Protection des Données |
| CQRS | Command Query Responsibility Segregation |
| DDD | Domain-Driven Design |

---

## 12. Points ouverts / à valider avec le porteur de projet

- Modèle de tarification SaaS (par praticien, par cabinet, freemium ?).
- Choix définitif de la solution de visioconférence pour la téléconsultation.
- Hébergeur cible et conformité (certification hébergement données de santé).
- Portée exacte de l'intégration eHealth en phase 5 (Réseau Santé Wallon / Vitalink / autre selon régions).
- Stratégie multi-tenant vs instance dédiée par cabinet.
- Priorisation définitive des professions cibles pour la Phase 6 (médecins généralistes/spécialistes, infirmiers, dentistes, psychologues) et calendrier associé.
- Cartographie détaillée, par profession candidate, des spécificités réglementaires et de nomenclature (ex. Sumehr pour les médecins) à intégrer avant ouverture.
- Impact de la généricité multi-professions sur le modèle de tarification SaaS (tarif différencié par profession ?).
