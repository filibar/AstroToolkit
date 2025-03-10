using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AstroToolkit.ViewModels
{
    public partial class WeatherViewModel : BaseViewModel
    {
        private readonly WeatherService _weatherService;
        private readonly LocationService _locationService;

        [ObservableProperty]
        private WeatherData _currentWeather;

        [ObservableProperty]
        private MoonPhase _currentMoonPhase;

        [ObservableProperty]
        private ObservableCollection<WeatherData> _hourlyForecast;

        [ObservableProperty]
        private ObservableCollection<WeatherData> _dailyForecast;

        [ObservableProperty]
        private double _currentLatitude;

        [ObservableProperty]
        private double _currentLongitude;

        [ObservableProperty]
        private string _locationName;

        [ObservableProperty]
        private bool _isGoodForAstrophotography;

        [ObservableProperty]
        private string _moonPhaseIconSource;

        public WeatherViewModel(WeatherService weatherService, LocationService locationService)
        {
            Title = "Weather";
            _weatherService = weatherService;
            _locationService = locationService;
            HourlyForecast = new ObservableCollection<WeatherData>();
            DailyForecast = new ObservableCollection<WeatherData>();
        }

        [RelayCommand]
        private async Task LoadWeatherDataAsync()
        {
            if (IsBusy)
                return;

            await ExecuteAsync(async () =>
            {
                // Get current location
                var status = await _locationService.CheckAndRequestLocationPermission();
                if (status != PermissionStatus.Granted)
                {
                    throw new Exception("Location permission is required to use this feature.");
                }

                var location = await _locationService.GetCurrentLocationAsync();
                if (location == null)
                {
                    throw new Exception("Could not get current location.");
                }

                CurrentLatitude = location.Latitude;
                CurrentLongitude = location.Longitude;

                // Get current weather and forecast
                CurrentWeather = await _weatherService.GetCurrentWeatherAsync(CurrentLatitude, CurrentLongitude);
                LocationName = CurrentWeather.LocationName;

                // Get moon phase
                CurrentMoonPhase = await _weatherService.GetMoonPhaseAsync(CurrentLatitude, CurrentLongitude);
                MoonPhaseIconSource = CurrentMoonPhase.GetPhaseIconResource();

                // Get forecast data
                var forecast = await _weatherService.GetWeatherForecastAsync(CurrentLatitude, CurrentLongitude);

                // Update collections
                HourlyForecast.Clear();
                foreach (var hourData in forecast.HourlyForecast.Take(24))
                {
                    HourlyForecast.Add(hourData);
                }

                DailyForecast.Clear();
                foreach (var dayData in forecast.DailyForecast)
                {
                    DailyForecast.Add(dayData);
                }

                // Determine if conditions are good for astrophotography
                IsGoodForAstrophotography = CurrentWeather.IsGoodForAstrophotography;

            }, "Failed to load weather data");
        }

        [RelayCommand]
        private void RefreshWeatherData()
        {
            LoadWeatherDataAsync().ConfigureAwait(false);
        }

        // Call this when the page appears
        public async Task InitializeAsync()
        {
            await LoadWeatherDataAsync();
        }
    }
}
