[← Sommaire](README.md)

# 4.3 Module Facturation & INAMI/MyCareNet

**Objectif** : sécuriser et automatiser le cycle de facturation belge basé sur la nomenclature INAMI (kinésithérapie en Phase 1, structure conçue pour accueillir la nomenclature d'autres professions — voir [4.6](04-6-extensibilite-multi-professions.md)).

Fonctionnalités attendues :
- Génération de factures basées sur les codes de nomenclature INAMI (voir `InamiCode` dans l'[architecture technique](../architecture_technique_saas.md)).
- Vérification d'assurabilité du patient via MyCareNet avant facturation.
- Gestion du tiers payant et du régime BIM/OMNIO.
- Télétransmission des attestations de soins (eAttest) vers les organismes assureurs via eHealth/MyCareNet.
- Suivi des statuts de télétransmission (envoyée, acceptée, rejetée) et gestion des rejets/corrections.
- Facturation patient (part non remboursée) avec génération de documents PDF conformes.
- Suivi des paiements, relances, et état des comptes patients.
- Journalisation immuable de toute émission ou modification de facture (exigence d'audit).
- Export comptable (format à définir avec l'expert-comptable du cabinet).

---
[← Agenda & Rendez-vous](04-2-agenda-rendez-vous.md) · [Sommaire](README.md) · [Suivant : Portail Patient & Téléconsultation →](04-4-portail-patient-teleconsultation.md)
