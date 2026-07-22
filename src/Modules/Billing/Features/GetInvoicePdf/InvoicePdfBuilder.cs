using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OmniCare.Modules.Billing.Features.GetInvoicePdf;

/// <summary>
/// Compose le document PDF d'une facture. Mise en page informative reprenant les
/// données de la facture, du praticien et du patient — <b>ce n'est pas une
/// reproduction du modèle d'attestation de soins prescrit par l'INAMI</b> (aucune
/// source officielle du gabarit exact n'a été confirmée à date de cette
/// implémentation, cf. ticket #38). À aligner sur le modèle officiel si l'attestation
/// doit un jour être générée telle quelle plutôt que télétransmise via eHealth.
/// </summary>
internal static class InvoicePdfBuilder
{
    public static byte[] Build(InvoicePdfData data)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Text(data.CabinetName).FontSize(16).Bold();
                    column.Item().Text($"{data.CabinetAddressLine}, {data.CabinetPostalCode} {data.CabinetCity}");
                    column.Item().Text($"N° d'entreprise (BCE) : {data.CabinetBceNumber}");
                    column.Item().PaddingTop(4).LineHorizontal(1);
                });

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Praticien").SemiBold();
                            col.Item().Text(data.PractitionerName);
                            col.Item().Text($"N° INAMI : {data.PractitionerInamiNumber}");
                            col.Item().Text($"Profession : {data.ProfessionLabel}");
                        });
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Patient").SemiBold();
                            col.Item().Text(data.PatientName);
                            if (!string.IsNullOrWhiteSpace(data.PatientAddressLine))
                                col.Item().Text($"{data.PatientAddressLine}, {data.PatientPostalCode} {data.PatientCity}");
                        });
                    });

                    column.Item().Text($"Facture n° {data.InvoiceId}").FontSize(12).Bold();
                    column.Item().Text($"Date d'émission : {data.IssuedOn:dd/MM/yyyy}");

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Acte").Bold();
                            header.Cell().Text("Code INAMI").Bold();
                            header.Cell().Text("Montant").Bold();
                        });

                        table.Cell().Text(data.ActLabel);
                        table.Cell().Text(data.ActCode);
                        table.Cell().Text($"{data.Total:F2} €");
                    });

                    column.Item().PaddingTop(6).Column(col =>
                    {
                        col.Item().Text($"Part patient (ticket modérateur) : {data.PatientShare:F2} €");
                        col.Item().Text($"Part organisme assureur : {data.MutualityShare:F2} €");
                        col.Item().Text($"Tiers payant appliqué : {(data.ThirdPartyPayer ? "Oui" : "Non")}");
                        col.Item().Text($"Statut : {data.Status}");
                        if (data.PaidOn is not null)
                            col.Item().Text($"Payé le {data.PaidOn:dd/MM/yyyy} ({data.PaymentMethod})");
                    });
                });

                page.Footer().AlignCenter()
                    .Text("Document généré à titre informatif — ne constitue pas l'attestation de soins officielle télétransmise à l'organisme assureur.")
                    .FontSize(8).Italic();
            });
        }).GeneratePdf();
    }
}
