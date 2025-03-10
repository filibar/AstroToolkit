using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AstroToolkit.ViewModels
{
    [QueryProperty(nameof(SpotId), "spotId")]
    public partial class SpotDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly WeatherService _weatherService;

        [ObservableProperty]
        private int _spotId;

        [ObservableProperty]
        private AstroSpot _spot;

        [ObservableProperty]
        private WeatherData _currentWeather;

        [ObservableProperty]
        private MoonPhase _currentMoonPhase;

        [ObservableProperty]
        private List<string> _examplePhotos;

        [ObservableProperty]
        private bool _isEditing;

        public SpotDetailViewModel(DatabaseService databaseService, WeatherService weatherService)
        {
            Title = "Spot Details";
            _databaseService = databaseService;
            _weatherService = weatherService;
        }

        // This is called when SpotId is set via the QueryProperty
        partial void OnSpotIdChanged(int value)
        {
            LoadSpotDataAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadSpotDataAsync()
        {
            if (IsBusy || SpotId <= 0)
                return;

            await ExecuteAsync(async () =>
            {
                Spot = await _databaseService.GetAstroSpotAsync(SpotId);
                
                if (Spot == null)
                {
                    throw new Exception($"Spot with ID {SpotId} not found.");
                }

                Title = Spot.Name;
                ExamplePhotos = Spot.GetExamplePhotosList();

                // Get weather at this location
                try 
                {
                    CurrentWeather = await _weatherService.GetCurrentWeatherAsync(Spot.Latitude, Spot.Longitude);
                    CurrentMoonPhase = await _weatherService.GetMoonPhaseAsync(Spot.Latitude, Spot.Longitude);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load weather data: {ex.Message}");
                    // Non-critical error, so we don't throw it
                }
            }, "Failed to load spot data");
        }

        [RelayCommand]
        private async Task NavigateToSpotAsync()
        {
            if (Spot == null)
                return;

            await ExecuteAsync(async () =>
            {
                // Generate Google Maps URL
                string url = Spot.GetGoogleMapsUrl();

                // Open in external navigation app
                await Launcher.OpenAsync(new Uri(url));
            }, "Failed to open navigation");
        }

        [RelayCommand]
        private void ToggleEdit()
        {
            IsEditing = !IsEditing;
        }

        [RelayCommand]
        private async Task SaveSpotAsync()
        {
            if (Spot == null)
                return;

            await ExecuteAsync(async () =>
            {
                await _databaseService.SaveAstroSpotAsync(Spot);
                IsEditing = false;
            }, "Failed to save spot");
        }

        [RelayCommand]
        private async Task UpdateRatingAsync(int rating)
        {
            if (Spot == null)
                return;

            await ExecuteAsync(async () =>
            {
                Spot.Rating = rating;
                await _databaseService.SaveAstroSpotAsync(Spot);
            }, "Failed to update rating");
        }

        [RelayCommand]
        private async Task AddPhotoAsync()
        {
            if (Spot == null)
                return;

            await ExecuteAsync(async () =>
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                if (result != null)
                {
                    // Read the file and convert to base64 for storage
                    var stream = await result.OpenReadAsync();
                    var bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                    string base64 = Convert.ToBase64String(bytes);

                    // Add to example photos
                    var photos = Spot.GetExamplePhotosList();
                    photos.Add(base64);
                    Spot.SetExamplePhotosList(photos);

                    // Update our viewmodel's list
                    ExamplePhotos = photos;

                    // Save to database
                    await _databaseService.SaveAstroSpotAsync(Spot);
                }
            }, "Failed to add photo");
        }

        [RelayCommand]
        private async Task RemovePhotoAsync(string photoData)
        {
            if (Spot == null || string.IsNullOrEmpty(photoData))
                return;

            await ExecuteAsync(async () =>
            {
                var photos = Spot.GetExamplePhotosList();
                photos.Remove(photoData);
                Spot.SetExamplePhotosList(photos);

                // Update our viewmodel's list
                ExamplePhotos = photos;

                // Save to database
                await _databaseService.SaveAstroSpotAsync(Spot);
            }, "Failed to remove photo");
        }
    }
}
