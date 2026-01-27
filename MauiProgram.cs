using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using JournalApp.Data;
using JournalApp.Repositories;
using JournalApp.Services;

namespace JournalApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddLogging(logging => 
        {
            logging.AddDebug();
        });
#endif

        // Database
        var dbPath = DbPathHelper.GetDatabasePath();
        builder.Services.AddDbContext<JournalDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Repositories
        builder.Services.AddScoped<IJournalRepository, JournalRepository>();
        builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();

        // Services
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<StreakService>();
        builder.Services.AddScoped<AnalyticsService>();
        builder.Services.AddScoped<ExportPdfService>();

        // Initialize database
        builder.Services.AddTransient<AppInitializer>();

        return builder.Build();
    }
}
