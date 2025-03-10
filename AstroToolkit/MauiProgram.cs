using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using AstroToolkit.Services;
using AstroToolkit.ViewModels;
using AstroToolkit.Views;

namespace AstroToolkit;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<LocationService>();
        builder.Services.AddSingleton<WeatherService>();
        builder.Services.AddSingleton<AstroCalculationService>();

        // Register view models
        builder.Services.AddTransient<MapViewModel>();
        builder.Services.AddTransient<WeatherViewModel>();
        builder.Services.AddTransient<ToolsViewModel>();
        builder.Services.AddTransient<SpotDetailViewModel>();

        // Register pages
        builder.Services.AddTransient<MapPage>();
        builder.Services.AddTransient<WeatherPage>();
        builder.Services.AddTransient<ToolsPage>();
        builder.Services.AddTransient<SpotDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
