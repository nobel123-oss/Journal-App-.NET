using Microsoft.EntityFrameworkCore;
using JournalApp.Data;
using JournalApp.Models;

namespace JournalApp.Repositories;

/// <summary>
/// Repository implementation for journal entry operations
/// Handles all database interactions for entries, moods, and tags
/// </summary>
public class JournalRepository : IJournalRepository
{
    private readonly JournalDbContext _context;

    public JournalRepository(JournalDbContext context)
    {
        _context = context;
    }

    #region Entry Operations

    public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
    {
        var dateOnly = date.Date;
        return await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.Date.Date == dateOnly);
    }

    public async Task<JournalEntry?> GetEntryByIdAsync(int id)
    {
        return await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<JournalEntry>> GetAllEntriesAsync()
    {
        return await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .Where(e => e.Date.Date >= start && e.Date.Date <= end)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllEntriesAsync();

        var lowerSearch = searchTerm.ToLower();

        return await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .Where(e => e.Title.ToLower().Contains(lowerSearch) || 
                        e.Content.ToLower().Contains(lowerSearch))
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> FilterEntriesAsync(
        DateTime? startDate, 
        DateTime? endDate, 
        List<int>? moodIds, 
        List<int>? tagIds)
    {
        var query = _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .AsQueryable();

        // Filter by date range
        if (startDate.HasValue)
        {
            var start = startDate.Value.Date;
            query = query.Where(e => e.Date.Date >= start);
        }

        if (endDate.HasValue)
        {
            var end = endDate.Value.Date;
            query = query.Where(e => e.Date.Date <= end);
        }

        // Filter by moods
        if (moodIds != null && moodIds.Any())
        {
            query = query.Where(e => 
                moodIds.Contains(e.PrimaryMoodId) ||
                (e.SecondaryMood1Id.HasValue && moodIds.Contains(e.SecondaryMood1Id.Value)) ||
                (e.SecondaryMood2Id.HasValue && moodIds.Contains(e.SecondaryMood2Id.Value)));
        }

        // Filter by tags
        if (tagIds != null && tagIds.Any())
        {
            query = query.Where(e => e.EntryTags.Any(et => tagIds.Contains(et.TagId)));
        }

        return await query
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<JournalEntry> CreateEntryAsync(JournalEntry entry)
    {
        entry.CreatedAt = DateTime.Now;
        entry.UpdatedAt = DateTime.Now;
        entry.Date = entry.Date.Date; // Ensure only date part

        _context.JournalEntries.Add(entry);
        await _context.SaveChangesAsync();
        
        return await GetEntryByIdAsync(entry.Id) ?? entry;
    }

    public async Task<JournalEntry> UpdateEntryAsync(JournalEntry entry)
    {
        entry.UpdatedAt = DateTime.Now;
        
        _context.JournalEntries.Update(entry);
        await _context.SaveChangesAsync();
        
        return await GetEntryByIdAsync(entry.Id) ?? entry;
    }

    public async Task DeleteEntryAsync(int id)
    {
        var entry = await _context.JournalEntries.FindAsync(id);
        if (entry != null)
        {
            _context.JournalEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> EntryExistsForDateAsync(DateTime date)
    {
        var dateOnly = date.Date;
        return await _context.JournalEntries.AnyAsync(e => e.Date.Date == dateOnly);
    }

    public async Task<(List<JournalEntry> Entries, int TotalCount)> GetEntriesPaginatedAsync(
        int pageNumber, 
        int pageSize)
    {
        var totalCount = await _context.JournalEntries.CountAsync();
        
        var entries = await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .OrderByDescending(e => e.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (entries, totalCount);
    }

    #endregion

    #region Mood Operations

    public async Task<List<Mood>> GetAllMoodsAsync()
    {
        return await _context.Moods
            .OrderBy(m => m.Category)
            .ThenBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Mood?> GetMoodByIdAsync(int id)
    {
        return await _context.Moods.FindAsync(id);
    }

    #endregion

    #region Tag Operations

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _context.Tags
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag?> GetTagByIdAsync(int id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteTagAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag != null && !tag.IsPrebuilt)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }

    #endregion
}
