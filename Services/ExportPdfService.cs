using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using JournalApp.Models;
using JournalApp.Repositories;
using PdfColors = QuestPDF.Helpers.Colors;

namespace JournalApp.Services;

/// <summary>
/// Service for exporting journal entries to PDF format
/// Uses QuestPDF library for PDF generation
/// </summary>
public class ExportPdfService
{
    private readonly IJournalRepository _repository;

    public ExportPdfService(IJournalRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Exports entries within a date range to a PDF file
    /// </summary>
    public async Task<string> ExportToPdfAsync(DateTime startDate, DateTime endDate, string filePath)
    {
        // Set QuestPDF license
        QuestPDF.Settings.License = LicenseType.Community;

        var entries = await _repository.GetEntriesByDateRangeAsync(startDate, endDate);

        if (!entries.Any())
        {
            throw new InvalidOperationException("No entries found in the selected date range.");
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(PdfColors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header()
                    .Text($"Journal Entries: {startDate:MMMM d, yyyy} - {endDate:MMMM d, yyyy}")
                    .SemiBold().FontSize(16).FontColor(PdfColors.Grey.Darken3);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(15);

                        foreach (var entry in entries)
                        {
                            column.Item().BorderBottom(1).BorderColor(PdfColors.Grey.Lighten2).PaddingBottom(15).Column(entryColumn =>
                            {
                                // Date
                                entryColumn.Item().Text(entry.Date.ToString("dddd, MMMM d, yyyy"))
                                    .SemiBold().FontSize(14).FontColor(PdfColors.Grey.Darken2);

                                // Title
                                entryColumn.Item().PaddingTop(5).Text(entry.Title)
                                    .Bold().FontSize(13);

                                // Mood
                                var moodText = $"Mood: {entry.PrimaryMood?.Emoji} {entry.PrimaryMood?.Name}";
                                if (entry.SecondaryMood1 != null)
                                    moodText += $", {entry.SecondaryMood1.Emoji} {entry.SecondaryMood1.Name}";
                                if (entry.SecondaryMood2 != null)
                                    moodText += $", {entry.SecondaryMood2.Emoji} {entry.SecondaryMood2.Name}";

                                entryColumn.Item().PaddingTop(3).Text(moodText)
                                    .FontSize(10).FontColor(PdfColors.Grey.Darken1);

                                // Tags
                                if (entry.EntryTags.Any())
                                {
                                    var tags = string.Join(", ", entry.EntryTags.Select(et => et.Tag?.Name));
                                    entryColumn.Item().PaddingTop(3).Text($"Tags: {tags}")
                                        .FontSize(10).FontColor(PdfColors.Grey.Darken1);
                                }

                                // Category
                                if (!string.IsNullOrEmpty(entry.Category))
                                {
                                    entryColumn.Item().PaddingTop(3).Text($"Category: {entry.Category}")
                                        .FontSize(10).FontColor(PdfColors.Grey.Darken1);
                                }

                                // Content
                                entryColumn.Item().PaddingTop(8).Text(entry.Content)
                                    .FontSize(11).LineHeight(1.5f);

                                // Word count and timestamps
                                entryColumn.Item().PaddingTop(8).Text($"Word Count: {entry.WordCount} | Created: {entry.CreatedAt:g}")
                                    .FontSize(9).Italic().FontColor(PdfColors.Grey.Medium);
                            });
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .DefaultTextStyle(x => x.FontSize(9))
                    .Text(t =>
                    {
                        t.Span("Page ");
                        t.CurrentPageNumber();
                        t.Span(" of ");
                        t.TotalPages();
                    });

            });
        });

        document.GeneratePdf(filePath);
        return filePath;
    }

    /// <summary>
    /// Gets suggested file name for export
    /// </summary>
    public string GetSuggestedFileName(DateTime startDate, DateTime endDate)
    {
        var fileName = $"Journal_{startDate:yyyy-MM-dd}_to_{endDate:yyyy-MM-dd}.pdf";
        return fileName;
    }
}
