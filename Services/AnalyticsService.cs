using JournalApp.Models;
using JournalApp.Repositories;

namespace JournalApp.Services;

/// <summary>
/// Service for generating analytics and insights from journal entries
/// </summary>
public class AnalyticsService
{
    private readonly IJournalRepository _repository;

    public AnalyticsService(IJournalRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Gets mood distribution (Positive, Neutral, Negative percentages)
    /// </summary>
    public async Task<Dictionary<string, double>> GetMoodDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetFilteredEntriesAsync(startDate, endDate);
        
        if (!entries.Any())
            return new Dictionary<string, double>
            {
                { "Positive", 0 },
                { "Neutral", 0 },
                { "Negative", 0 }
            };

        var totalMoods = entries.Count;
        var moodCategories = entries
            .GroupBy(e => e.PrimaryMood?.Category ?? "Unknown")
            .ToDictionary(g => g.Key, g => (double)g.Count() / totalMoods * 100);

        return new Dictionary<string, double>
        {
            { "Positive", moodCategories.GetValueOrDefault("Positive", 0) },
            { "Neutral", moodCategories.GetValueOrDefault("Neutral", 0) },
            { "Negative", moodCategories.GetValueOrDefault("Negative", 0) }
        };
    }

    /// <summary>
    /// Gets the most frequent mood
    /// </summary>
    public async Task<(string MoodName, int Count)?> GetMostFrequentMoodAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetFilteredEntriesAsync(startDate, endDate);
        
        if (!entries.Any())
            return null;

        var moodCount = entries
            .GroupBy(e => e.PrimaryMood?.Name ?? "Unknown")
            .Select(g => new { MoodName = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        return moodCount != null ? (moodCount.MoodName, moodCount.Count) : null;
    }

    /// <summary>
    /// Gets the most used tags
    /// </summary>
    public async Task<List<(string TagName, int Count)>> GetMostUsedTagsAsync(int topN = 10, DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetFilteredEntriesAsync(startDate, endDate);
        
        if (!entries.Any())
            return new List<(string, int)>();

        var tagCounts = entries
            .SelectMany(e => e.EntryTags)
            .GroupBy(et => et.Tag?.Name ?? "Unknown")
            .Select(g => (TagName: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count)
            .Take(topN)
            .ToList();

        return tagCounts;
    }

    /// <summary>
    /// Gets tag breakdown (percentage of entries per tag)
    /// </summary>
    public async Task<Dictionary<string, double>> GetTagBreakdownAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetFilteredEntriesAsync(startDate, endDate);
        
        if (!entries.Any())
            return new Dictionary<string, double>();

        var totalEntries = entries.Count;
        var tagBreakdown = entries
            .SelectMany(e => e.EntryTags.Select(et => et.Tag?.Name ?? "Unknown"))
            .GroupBy(t => t)
            .ToDictionary(
                g => g.Key,
                g => (double)g.Count() / totalEntries * 100
            );

        return tagBreakdown;
    }

    /// <summary>
    /// Gets word count trends over time (average per time period)
    /// </summary>
    public async Task<Dictionary<DateTime, double>> GetWordCountTrendsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetFilteredEntriesAsync(startDate, endDate);
        
        if (!entries.Any())
            return new Dictionary<DateTime, double>();

        // Group by month and calculate average word count
        var trends = entries
            .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Average(e => e.WordCount)
            );

        return trends;
    }

    /// <summary>
    /// Gets average word count
    /// </summary>
    public async Task<double> GetAverageWordCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetFilteredEntriesAsync(startDate, endDate);
        
        if (!entries.Any())
            return 0;

        return entries.Average(e => e.WordCount);
    }

    /// <summary>
    /// Gets total word count
    /// </summary>
    public async Task<int> GetTotalWordCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetFilteredEntriesAsync(startDate, endDate);
        return entries.Sum(e => e.WordCount);
    }

    /// <summary>
    /// Gets mood counts for all moods
    /// </summary>
    public async Task<Dictionary<string, int>> GetAllMoodCountsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetFilteredEntriesAsync(startDate, endDate);
        
        if (!entries.Any())
            return new Dictionary<string, int>();

        var moodCounts = entries
            .GroupBy(e => e.PrimaryMood?.Name ?? "Unknown")
            .ToDictionary(g => g.Key, g => g.Count());

        return moodCounts;
    }

    /// <summary>
    /// Helper method to get filtered entries
    /// </summary>
    private async Task<List<JournalEntry>> GetFilteredEntriesAsync(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue || endDate.HasValue)
        {
            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.MaxValue;
            return await _repository.GetEntriesByDateRangeAsync(start, end);
        }

        return await _repository.GetAllEntriesAsync();
    }
}
