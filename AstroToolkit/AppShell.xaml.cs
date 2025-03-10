namespace AstroToolkit;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(SpotDetailPage), typeof(Views.SpotDetailPage));
    }
}
