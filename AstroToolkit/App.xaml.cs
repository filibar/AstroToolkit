namespace AstroToolkit;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        // Custom settings for better user experience in dark environments
        window.Page.RequestedThemeChanged += (s, e) => {
            // Apply dark theme optimizations when in dark mode
        };

        return window;
    }

    protected override void OnStart()
    {
        // Initialize any services that need to start with the app
        base.OnStart();
    }

    protected override void OnSleep()
    {
        // Handle app sleeping (background) to preserve battery
        base.OnSleep();
    }

    protected override void OnResume()
    {
        // Handle app resuming
        base.OnResume();
    }
}
