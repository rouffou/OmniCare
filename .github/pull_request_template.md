## Objet

<!-- Quel sujet (une seule chose par PR) ? Lien vers la section du cahier des charges si pertinent. -->

## Type

- [ ] `feat` — nouvelle fonctionnalité / slice
- [ ] `fix` — correction
- [ ] `refactor` / `chore` / `docs` / `test`
- [ ] `hotfix` — correction critique (base `main`)

## Checklist

- [ ] `dotnet build OmniCare.slnx` vert (0 warning)
- [ ] `dotnet test OmniCare.slnx` vert
- [ ] Vocabulaire du Domain générique (pas de terme spécifique kiné — cf. CLAUDE.md)
- [ ] Commandes d'écriture : `ITransactionalRequest` + pas de `SaveChangesAsync` dans le handler
- [ ] Écriture dossier médical / facture : commande marquée `IAuditableRequest`
- [ ] Aucune donnée de santé réelle (code, tests, fixtures)
