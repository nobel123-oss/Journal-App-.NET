using JournalApp.Data;

namespace JournalApp;

public partial class App : Application
{
    private readonly AppInitializer _appInitializer;

    public App(AppInitializer appInitializer)
    {
        InitializeComponent();
        _appInitializer = appInitializer;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Initialize database and seed data
        _appInitializer.Initialize();

        return new Window(new MainPage());
    }
}
