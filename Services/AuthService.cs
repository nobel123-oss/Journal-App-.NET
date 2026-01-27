using System.Security.Cryptography;
using System.Text;
using JournalApp.Repositories;

namespace JournalApp.Services;

/// <summary>
/// Service for authentication and password management
/// Uses SHA256 for password hashing
/// </summary>
public class AuthService
{
    private readonly ISettingsRepository _settingsRepository;
    private bool _isAuthenticated = false;

    public AuthService(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public bool IsAuthenticated => _isAuthenticated;

    /// <summary>
    /// Checks if a password is set
    /// </summary>
    public async Task<bool> HasPasswordAsync()
    {
        var settings = await _settingsRepository.GetSettingsAsync();
        return !string.IsNullOrEmpty(settings.PasswordHash);
    }

    /// <summary>
    /// Sets a new password
    /// </summary>
    public async Task SetPasswordAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty");

        var settings = await _settingsRepository.GetSettingsAsync();
        settings.PasswordHash = HashPassword(password);
        settings.IsLocked = true;
        await _settingsRepository.UpdateSettingsAsync(settings);
    }

    /// <summary>
    /// Validates the entered password
    /// </summary>
    public async Task<bool> ValidatePasswordAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        var settings = await _settingsRepository.GetSettingsAsync();
        
        if (string.IsNullOrEmpty(settings.PasswordHash))
            return false;

        var hashedInput = HashPassword(password);
        var isValid = hashedInput == settings.PasswordHash;

        if (isValid)
        {
            _isAuthenticated = true;
        }

        return isValid;
    }

    /// <summary>
    /// Removes the password protection
    /// </summary>
    public async Task RemovePasswordAsync()
    {
        var settings = await _settingsRepository.GetSettingsAsync();
        settings.PasswordHash = null;
        settings.IsLocked = false;
        await _settingsRepository.UpdateSettingsAsync(settings);
        _isAuthenticated = true;
    }

    /// <summary>
    /// Locks the application
    /// </summary>
    public void Lock()
    {
        _isAuthenticated = false;
    }

    /// <summary>
    /// Checks if the app is locked
    /// </summary>
    public async Task<bool> IsLockedAsync()
    {
        var hasPassword = await HasPasswordAsync();
        return hasPassword && !_isAuthenticated;
    }

    /// <summary>
    /// Hashes a password using SHA256
    /// </summary>
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
