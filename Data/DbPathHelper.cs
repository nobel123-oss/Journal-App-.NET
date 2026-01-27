namespace JournalApp.Data;

/// <summary>
/// Helper class to determine the SQLite database file path
/// </summary>
public static class DbPathHelper
{
    /// <summary>
    /// Gets the full path to the SQLite database file
    /// </summary>
    /// <returns>Database file path in the application data directory</returns>
    public static string GetDatabasePath()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        var dbPath = Path.Combine(appDataPath, "journal.db");
        return dbPath;
    }
}
