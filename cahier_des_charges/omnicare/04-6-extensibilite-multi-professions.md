[← Sommaire](README.md)

# 4.6 Extensibilité multi-professions de santé (vision cible, activation progressive)

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
[← Intégration eHealth](04-5-integration-ehealth.md) · [Sommaire](README.md) · [Suivant : Exigences non fonctionnelles →](05-exigences-non-fonctionnelles.md)
