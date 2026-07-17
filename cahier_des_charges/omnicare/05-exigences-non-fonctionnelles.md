[← Sommaire](README.md)

# 5. Exigences non fonctionnelles

## 5.1 Sécurité & confidentialité
- Chiffrement des données de santé au repos et en transit (TLS, chiffrement base de données).
- Authentification forte (MFA) pour les praticiens et le personnel administratif.
- Gestion fine des droits d'accès par rôle (praticien, secrétariat, administrateur, patient).
- Audit trail systématique sur toute action touchant un dossier médical ou une facture (déjà prévu au niveau architecture via les Pipeline Behaviors Mediarq).

## 5.2 Conformité réglementaire
- Conformité RGPD (données de santé = catégorie particulière, art. 9).
- Respect du secret médical et des recommandations de l'INAMI.
- Hébergement des données de santé chez un hébergeur certifié / conforme à la réglementation belge en vigueur.
- Politique de conservation et de purge des données conforme aux durées légales de conservation des dossiers médicaux.

## 5.3 Disponibilité & performance
- Disponibilité cible du service : à définir (SLA), avec attention particulière à l'agenda et à la facturation (usage quotidien critique).
- Temps de réponse acceptable sur les opérations courantes (consultation agenda, ouverture dossier patient).
- Plan de sauvegarde et de reprise après sinistre (RPO/RTO à définir).

## 5.4 Scalabilité & évolutivité
- Architecture modulaire (Vertical Slice) permettant l'ajout de nouveaux modules métier sans régression sur l'existant.
- Capacité à supporter la montée en charge multi-cabinets (multi-tenant à confirmer).
- **Généricité du modèle métier** : dossier médical, nomenclature d'actes et rôles praticien conçus dès la Phase 1 comme paramétrables par profession de santé, afin de permettre l'onboarding d'autres professions sans refonte du socle (voir [4.6](04-6-extensibilite-multi-professions.md)).

## 5.5 Utilisabilité
- Interface simple et accessible pour des praticiens non technophiles.
- Portail patient accessible et responsive (mobile-first recommandé).
- Support multilingue à minima FR/NL (contexte belge), EN en option.

---
[← Extensibilité multi-professions](04-6-extensibilite-multi-professions.md) · [Sommaire](README.md) · [Suivant : Architecture technique →](06-architecture-technique.md)
