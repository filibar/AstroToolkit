using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AstroToolkitWeb.Services;
using AstroToolkitWeb.Models;

namespace AstroToolkitWeb.Pages
{
    public class WeatherModel : PageModel
    {
        private readonly ILogger<WeatherModel> _logger;
        private readonly WeatherService _weatherService;
        private readonly AstroCalculationService _astroService;
        private readonly IConfiguration _configuration;

        // Properties for query string parameters
        [BindProperty(SupportsGet = true)]
        public double? Lat { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public double? Lon { get; set; }

        // Properties for weather data
        public WeatherData? CurrentWeather { get; private set; }
        public IEnumerable<WeatherData>? Forecast { get; private set; }
        public MoonPhase? MoonPhase { get; private set; }

        public WeatherModel(
            ILogger<WeatherModel> logger,
            WeatherService weatherService,
            AstroCalculationService astroService,
            IConfiguration configuration)
        {
            _logger = logger;
            _weatherService = weatherService;
            _astroService = astroService;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            try
            {
                // Check if we have coordinates from the query string
                if (Lat.HasValue && Lon.HasValue)
                {
                    // Load weather data for the provided coordinates
                    CurrentWeather = await _weatherService.GetWeatherDataAsync(Lat.Value, Lon.Value);
                    
                    // Get forecast for next 5 days
                    Forecast = new List<WeatherData>();
                    for (int i = 1; i <= 5; i++)
                    {
                        var forecastData = await _weatherService.GetWeatherDataAsync(Lat.Value, Lon.Value, DateTime.UtcNow.AddDays(i));
                        if (forecastData != null)
                        {
                            ((List<WeatherData>)Forecast).Add(forecastData);
                        }
                    }
                }
                
                // Calculate moon phase for today
                MoonPhase = _astroService.CalculateMoonPhase(DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading weather data for coordinates: {Lat}, {Lon}", Lat, Lon);
            }
        }
    }
}