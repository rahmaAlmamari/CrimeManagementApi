using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CrimeManagementApi.Models;
using System.Linq;

namespace CrimeManagementApi.Utilities
{
    /// <summary>
    /// Generates professional PDF reports for crime cases,
    /// including case details, participants, evidence with audit history, and comments.
    /// </summary>
    public static class PdfGenerator
    {
        /// <summary>
        /// Builds a complete case report as a PDF byte array.
        /// </summary>
        public static byte[] GenerateCaseReport(Case caseEntity)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(11).FontFamily("Arial"));

                    // ======================================================
                    // 🔹 HEADER
                    // ======================================================
                    page.Header().Row(row =>
                    {
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("Crime Management System")
                                .Bold().FontSize(18).FontColor(Colors.Blue.Medium);
                            col.Item().Text($"Case Report – {caseEntity.CaseNumber}")
                                .FontSize(13).FontColor(Colors.Grey.Darken2);
                        });
                    });

                    // ======================================================
                    // 🔹 MAIN CONTENT
                    // ======================================================
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // 🔸 CASE INFO
                        col.Item().Text("Case Information").Bold().FontSize(16);
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().Text($"Case Name: {caseEntity.Name}");
                        col.Item().Text($"Case Type: {caseEntity.CaseType}");
                        col.Item().Text($"Status: {caseEntity.Status}");
                        col.Item().Text($"Authorization Level: {caseEntity.ClearanceLevel}");
                        col.Item().Text($"Area/City: {caseEntity.AreaCity}");
                        col.Item().Text($"Description: {caseEntity.Description}");
                        col.Item().Text($"Created At: {caseEntity.CreatedAt:dd MMM yyyy HH:mm}");

                        // 🔸 CASE CREATOR
                        if (caseEntity.CreatedByUser != null)
                        {
                            col.Item().PaddingVertical(10).Element(e =>
                            {
                                e.Border(1).BorderColor(Colors.Grey.Lighten1)
                                 .Background(Colors.Grey.Lighten4)
                                 .Padding(10)
                                 .Column(inner =>
                                 {
                                     inner.Item().Text("👤 Case Creator Information")
                                         .Bold().FontSize(14).FontColor(Colors.Blue.Medium);
                                     inner.Item().Text($"Name: {caseEntity.CreatedByUser.FullName}");
                                     inner.Item().Text($"User ID: {caseEntity.CreatedByUser.Id}");
                                     inner.Item().Text($"Role: {caseEntity.CreatedByUser.Role}");
                                 });
                            });
                        }

                        // ======================================================
                        // 🔹 PARTICIPANTS SECTION
                        // ======================================================
                        if (caseEntity.Participants?.Any() == true)
                        {
                            col.Item().PaddingTop(15).Text("Participants").Bold().FontSize(16);
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                            var grouped = caseEntity.Participants
                                .Where(cp => cp.Participant != null)
                                .GroupBy(cp => cp.Participant.Role)
                                .OrderBy(g => g.Key);

                            foreach (var group in grouped)
                            {
                                col.Item().PaddingTop(5)
                                    .Text($"{group.Key}s")
                                    .Bold().FontColor(Colors.Blue.Medium);

                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(30);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(3);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("#").Bold();
                                        header.Cell().Text("Full Name").Bold();
                                        header.Cell().Text("Phone").Bold();
                                        header.Cell().Text("Added By").Bold();
                                        header.Cell().Text("Linked At").Bold();
                                    });

                                    int index = 1;
                                    foreach (var cp in group)
                                    {
                                        table.Cell().Text(index++.ToString());
                                        table.Cell().Text(cp.Participant!.FullName);
                                        table.Cell().Text(cp.Participant.Phone ?? "-");
                                        table.Cell().Text(cp.Participant.AddedByUser?.FullName ?? "Unknown");
                                        table.Cell().Text(cp.LinkedAt.ToString("dd/MM/yyyy"));
                                    }
                                });
                            }
                        }
                        else
                        {
                            col.Item().PaddingTop(10)
                                .Text("No participants linked to this case.")
                                .Italic().FontColor(Colors.Grey.Darken1);
                        }

                        // ======================================================
                        // 🔹 EVIDENCE SECTION (with audit trail)
                        // ======================================================
                        if (caseEntity.EvidenceList?.Any() == true)
                        {
                            col.Item().PaddingTop(15).Text("Evidence Summary").Bold().FontSize(16);
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                            foreach (var ev in caseEntity.EvidenceList.OrderBy(e => e.Id))
                            {
                                col.Item().PaddingTop(10).Element(evidenceBlock =>
                                {
                                    evidenceBlock.Border(1).BorderColor(Colors.Grey.Lighten2)
                                        .Background(Colors.Grey.Lighten5)
                                        .Padding(10)
                                        .Column(inner =>
                                        {
                                            inner.Item().Text($"🔍 {ev.Type}")
                                                .Bold().FontSize(13).FontColor(Colors.Blue.Medium);
                                            inner.Item().Text($"Description: {ev.Description ?? "-"}");
                                            inner.Item().Text($"File Path: {ev.FilePath ?? "-"}");
                                            inner.Item().Text($"Mime Type: {ev.MimeType ?? "-"}");
                                            inner.Item().Text($"Added At: {ev.AddedAt:dd/MM/yyyy HH:mm}");
                                            inner.Item().Text($"Added By: {ev.AddedByUser?.FullName ?? "System"}");
                                            inner.Item().Text($"Remarks: {ev.Remarks ?? "-"}");

                                            inner.Item().PaddingTop(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                                            // 🔸 Include audit logs if available
                                            if (ev.AuditLogs?.Any() == true)
                                            {
                                                inner.Item().PaddingTop(5).Text("Audit Trail")
                                                    .Bold().FontSize(12).FontColor(Colors.Blue.Medium);

                                                inner.Item().Table(table =>
                                                {
                                                    table.ColumnsDefinition(columns =>
                                                    {
                                                        columns.ConstantColumn(30);
                                                        columns.RelativeColumn(2);
                                                        columns.RelativeColumn(5);
                                                        columns.RelativeColumn(3);
                                                        columns.RelativeColumn(3);
                                                    });

                                                    table.Header(header =>
                                                    {
                                                        header.Cell().Text("#").Bold();
                                                        header.Cell().Text("Action").Bold();
                                                        header.Cell().Text("Details").Bold();
                                                        header.Cell().Text("User").Bold();
                                                        header.Cell().Text("Time").Bold();
                                                    });

                                                    int logIndex = 1;
                                                    foreach (var log in ev.AuditLogs.OrderBy(a => a.ActedAt))
                                                    {
                                                        table.Cell().Text(logIndex++.ToString());
                                                        table.Cell().Text(log.Action);
                                                        table.Cell().Text(log.Details ?? "-");
                                                        table.Cell().Text(log.ActedByUser?.FullName ?? "System");
                                                        table.Cell().Text(log.ActedAt.ToString("dd/MM/yyyy HH:mm"));
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                inner.Item().PaddingTop(5)
                                                    .Text("No audit records for this evidence.")
                                                    .Italic().FontColor(Colors.Grey.Darken1);
                                            }
                                        });
                                });
                            }
                        }
                        else
                        {
                            col.Item().PaddingTop(10)
                                .Text("No evidence records found.")
                                .Italic().FontColor(Colors.Grey.Darken1);
                        }

                        // ======================================================
                        // 🔹 COMMENTS SECTION
                        // ======================================================
                        if (caseEntity.CaseComments?.Any() == true)
                        {
                            col.Item().PaddingTop(15).Text("Comments").Bold().FontSize(16);
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                            foreach (var comment in caseEntity.CaseComments.OrderBy(c => c.CreatedAt))
                            {
                                col.Item().PaddingVertical(5).Element(e =>
                                {
                                    e.Border(1).BorderColor(Colors.Grey.Lighten2)
                                     .Background(Colors.Grey.Lighten5)
                                     .Padding(8)
                                     .Column(inner =>
                                     {
                                         inner.Item().Text($"{comment.User?.FullName ?? "Unknown"}")
                                             .Bold().FontColor(Colors.Blue.Medium);
                                         inner.Item().Text(comment.Content);
                                         inner.Item().Text($"— {comment.CreatedAt:dd MMM yyyy HH:mm}")
                                             .FontSize(10).FontColor(Colors.Grey.Darken1);
                                     });
                                });
                            }
                        }
                        else
                        {
                            col.Item().PaddingTop(10)
                                .Text("No comments available for this case.")
                                .Italic().FontColor(Colors.Grey.Darken1);
                        }
                    });

                    // ======================================================
                    // 🔹 FOOTER
                    // ======================================================
                    page.Footer().AlignCenter().Text(
                        $"Generated automatically on {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                });
            }).GeneratePdf();
        }
    }
}
