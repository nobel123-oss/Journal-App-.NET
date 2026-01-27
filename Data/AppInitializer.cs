using Microsoft.EntityFrameworkCore;
using JournalApp.Models;

namespace JournalApp.Data;

/// <summary>
/// Initializes the application database and seeds initial data
/// </summary>
public class AppInitializer
{
    private readonly JournalDbContext _context;

    public AppInitializer(JournalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Ensures database is created and seeds initial data
    /// </summary>
    public void Initialize()
    {
        try
        {
            // Ensure database is created
            _context.Database.EnsureCreated();

            // Seed moods if not exists
            if (!_context.Moods.Any())
            {
                var moods = SeedData.GetMoods();
                _context.Moods.AddRange(moods);
                _context.SaveChanges();
            }

            // Seed tags if not exists
            if (!_context.Tags.Any())
            {
                var tags = SeedData.GetTags();
                _context.Tags.AddRange(tags);
                _context.SaveChanges();
            }

            // Create default app settings if not exists
            if (!_context.AppSettings.Any())
            {
                var settings = new AppSettings
                {
                    Theme = "Light",
                    IsLocked = false,
                    PasswordHash = null
                };
                _context.AppSettings.Add(settings);
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            // Log error but don't crash the app
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }
}
