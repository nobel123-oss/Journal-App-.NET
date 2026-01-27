using JournalApp.Repositories;

namespace JournalApp.Services;

/// <summary>
/// Service for calculating journaling streaks and missed days
/// </summary>
public class StreakService
{
    private readonly IJournalRepository _repository;

    public StreakService(IJournalRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Calculates the current daily streak
    /// </summary>
    public async Task<int> GetCurrentStreakAsync()
    {
        var entries = await _repository.GetAllEntriesAsync();
        
        if (!entries.Any())
            return 0;

        var sortedDates = entries
            .Select(e => e.Date.Date)
            .OrderByDescending(d => d)
            .ToList();

        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);

        // Check if there's an entry today or yesterday to start the streak
        if (!sortedDates.Contains(today) && !sortedDates.Contains(yesterday))
            return 0;

        int streak = 0;
        var currentDate = sortedDates.Contains(today) ? today : yesterday;

        foreach (var date in sortedDates)
        {
            if (date == currentDate)
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }
            else if (date < currentDate)
            {
                break;
            }
        }

        return streak;
    }

    /// <summary>
    /// Calculates the longest streak ever achieved
    /// </summary>
    public async Task<int> GetLongestStreakAsync()
    {
        var entries = await _repository.GetAllEntriesAsync();
        
        if (!entries.Any())
            return 0;

        var sortedDates = entries
            .Select(e => e.Date.Date)
            .OrderBy(d => d)
            .ToList();

        int longestStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < sortedDates.Count; i++)
        {
            var diff = (sortedDates[i] - sortedDates[i - 1]).Days;

            if (diff == 1)
            {
                currentStreak++;
                longestStreak = Math.Max(longestStreak, currentStreak);
            }
            else
            {
                currentStreak = 1;
            }
        }

        return longestStreak;
    }

    /// <summary>
    /// Gets a list of dates with no entries within a date range
    /// </summary>
    public async Task<List<DateTime>> GetMissedDaysAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var start = startDate?.Date ?? DateTime.Today.AddMonths(-1);
        var end = endDate?.Date ?? DateTime.Today;

        var entries = await _repository.GetEntriesByDateRangeAsync(start, end);
        var entryDates = entries.Select(e => e.Date.Date).ToHashSet();

        var missedDays = new List<DateTime>();
        
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (!entryDates.Contains(date))
            {
                missedDays.Add(date);
            }
        }

        return missedDays;
    }

    /// <summary>
    /// Gets the total number of entries
    /// </summary>
    public async Task<int> GetTotalEntriesAsync()
    {
        var entries = await _repository.GetAllEntriesAsync();
        return entries.Count;
    }
}
