using JournalApp.Models;

namespace JournalApp.Repositories;

/// <summary>
/// Interface for application settings repository operations
/// </summary>
public interface ISettingsRepository
{
    Task<AppSettings> GetSettingsAsync();
    Task<AppSettings> UpdateSettingsAsync(AppSettings settings);
}
