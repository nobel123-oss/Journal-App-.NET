using JournalApp.Models;

namespace JournalApp.Repositories;

/// <summary>
/// Interface for journal entry repository operations
/// </summary>
public interface IJournalRepository
{
    // Entry operations
    Task<JournalEntry?> GetEntryByDateAsync(DateTime date);
    Task<JournalEntry?> GetEntryByIdAsync(int id);
    Task<List<JournalEntry>> GetAllEntriesAsync();
    Task<List<JournalEntry>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm);
    Task<List<JournalEntry>> FilterEntriesAsync(DateTime? startDate, DateTime? endDate, List<int>? moodIds, List<int>? tagIds);
    Task<JournalEntry> CreateEntryAsync(JournalEntry entry);
    Task<JournalEntry> UpdateEntryAsync(JournalEntry entry);
    Task DeleteEntryAsync(int id);
    Task<bool> EntryExistsForDateAsync(DateTime date);
    
    // Pagination
    Task<(List<JournalEntry> Entries, int TotalCount)> GetEntriesPaginatedAsync(int pageNumber, int pageSize);

    // Mood operations
    Task<List<Mood>> GetAllMoodsAsync();
    Task<Mood?> GetMoodByIdAsync(int id);

    // Tag operations
    Task<List<Tag>> GetAllTagsAsync();
    Task<Tag?> GetTagByIdAsync(int id);
    Task<Tag?> GetTagByNameAsync(string name);
    Task<Tag> CreateTagAsync(Tag tag);
    Task DeleteTagAsync(int id);
}
