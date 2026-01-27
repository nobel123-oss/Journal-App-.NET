using Microsoft.EntityFrameworkCore;
using JournalApp.Data;
using JournalApp.Models;

namespace JournalApp.Repositories;

/// <summary>
/// Repository implementation for application settings operations
/// </summary>
public class SettingsRepository : ISettingsRepository
{
    private readonly JournalDbContext _context;

    public SettingsRepository(JournalDbContext context)
    {
        _context = context;
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        var settings = await _context.AppSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            settings = new AppSettings
            {
                Theme = "Light",
                IsLocked = false,
                PasswordHash = null
            };
            _context.AppSettings.Add(settings);
            await _context.SaveChangesAsync();
        }

        return settings;
    }

    public async Task<AppSettings> UpdateSettingsAsync(AppSettings settings)
    {
        _context.AppSettings.Update(settings);
        await _context.SaveChangesAsync();
        return settings;
    }
}
