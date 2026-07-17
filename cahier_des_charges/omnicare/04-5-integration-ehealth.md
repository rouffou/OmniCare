[← Sommaire](README.md)

# 4.5 Intégration eHealth (phase ultérieure)

**Objectif** : anticiper l'interopérabilité complète avec l'écosystème eHealth belge, au-delà de MyCareNet.

Éléments à intégrer progressivement :
- Authentification forte via eHealth (certificats, eID).
- Connexion au Réseau Santé Wallon / Vitalink / Intermed (selon la région) pour le partage de données patient inter-praticiens.
- Sumehr (Résumé électronique du dossier médical) si pertinent pour la coordination des soins.
- Consentement patient centralisé (plateforme de consentement eHealth).
- Veille réglementaire continue, les exigences eHealth évoluant régulièrement.

Cette intégration est posée comme une extension du module Facturation/MyCareNet déjà prévu dans l'[architecture technique](../architecture_technique_saas.md) (`IMyCareNetService`), afin de limiter la dette technique lors de son implémentation future.

---
[← Portail Patient & Téléconsultation](04-4-portail-patient-teleconsultation.md) · [Sommaire](README.md) · [Suivant : Extensibilité multi-professions →](04-6-extensibilite-multi-professions.md)
