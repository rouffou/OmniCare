[← Sommaire](README.md)

# 4.1 Module Patients & Dossiers médicaux

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
- Le dossier médical est un sous-ensemble protégé nécessitant une traçabilité d'accès renforcée (audit trail — cf. [architecture technique](../architecture_technique_saas.md), section Pipeline Behaviors).
- Séparation claire entre données administratives (accessibles au secrétariat) et données cliniques (réservées au praticien).

---
[← Périmètre du projet](03-perimetre-projet.md) · [Sommaire](README.md) · [Suivant : Agenda & Rendez-vous →](04-2-agenda-rendez-vous.md)
