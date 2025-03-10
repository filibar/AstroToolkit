using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Maui;

namespace AstroToolkit.ViewModels
{
    public partial class MapViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly LocationService _locationService;
        private Map _map;

        [ObservableProperty]
        private ObservableCollection<AstroSpot> _astroSpots;

        [ObservableProperty]
        private AstroSpot _selectedSpot;
        
        [ObservableProperty]
        private bool _isAddingNewSpot;
        
        [ObservableProperty]
        private double _currentLatitude;
        
        [ObservableProperty]
        private double _currentLongitude;

        [ObservableProperty]
        private bool _isLightPollutionLayerVisible;

        public Map MapInstance => _map;

        public MapViewModel(DatabaseService databaseService, LocationService locationService)
        {
            Title = "Astro Map";
            _databaseService = databaseService;
            _locationService = locationService;
            AstroSpots = new ObservableCollection<AstroSpot>();
            InitializeMap();
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            await ExecuteAsync(async () =>
            {
                var spotsList = await _databaseService.GetAllAstroSpotsAsync();
                AstroSpots.Clear();
                foreach (var spot in spotsList)
                {
                    AstroSpots.Add(spot);
                }
                await UpdateMapPins();
            }, "Failed to load astro spots");
        }

        [RelayCommand]
        private async Task GetCurrentLocationAsync()
        {
            if (IsBusy)
                return;

            await ExecuteAsync(async () =>
            {
                var status = await _locationService.CheckAndRequestLocationPermission();
                if (status != PermissionStatus.Granted)
                {
                    throw new Exception("Location permission is required to use this feature.");
                }

                var location = await _locationService.GetCurrentLocationAsync();
                if (location != null)
                {
                    CurrentLatitude = location.Latitude;
                    CurrentLongitude = location.Longitude;
                    
                    // Center map on current location
                    _map.Navigator.CenterOn(SphericalMercator.FromLonLat(location.Longitude, location.Latitude).ToMPoint());
                    _map.Navigator.ZoomTo(13);
                }
                else
                {
                    throw new Exception("Could not get current location.");
                }
            }, "Failed to get current location");
        }

        [RelayCommand]
        private async Task ToggleLightPollutionLayerAsync()
        {
            IsLightPollutionLayerVisible = !IsLightPollutionLayerVisible;
            UpdateLightPollutionLayer();
        }

        [RelayCommand]
        private async Task AddNewSpotAsync()
        {
            IsAddingNewSpot = true;
            
            // Temporarily set SelectedSpot to a new instance for adding
            SelectedSpot = new AstroSpot
            {
                Latitude = CurrentLatitude,
                Longitude = CurrentLongitude,
                CreatedDate = DateTime.Now,
                Name = "New Spot",
                Rating = 0
            };
        }

        [RelayCommand]
        private async Task SaveSpotAsync()
        {
            if (SelectedSpot == null)
                return;

            await ExecuteAsync(async () =>
            {
                await _databaseService.SaveAstroSpotAsync(SelectedSpot);
                
                // If this is a new spot, add it to the collection
                if (!AstroSpots.Contains(SelectedSpot))
                {
                    AstroSpots.Add(SelectedSpot);
                }
                
                IsAddingNewSpot = false;
                SelectedSpot = null;
                await UpdateMapPins();
            }, "Failed to save spot");
        }

        [RelayCommand]
        private async Task CancelAddSpotAsync()
        {
            IsAddingNewSpot = false;
            SelectedSpot = null;
        }

        [RelayCommand]
        private async Task DeleteSpotAsync(AstroSpot spot)
        {
            if (spot == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Deletion", 
                $"Are you sure you want to delete '{spot.Name}'?", 
                "Yes", "No");

            if (!confirm)
                return;

            await ExecuteAsync(async () =>
            {
                await _databaseService.DeleteAstroSpotAsync(spot);
                AstroSpots.Remove(spot);
                
                if (SelectedSpot == spot)
                {
                    SelectedSpot = null;
                }
                
                await UpdateMapPins();
            }, "Failed to delete spot");
        }

        [RelayCommand]
        private async Task RateSpotAsync(int rating)
        {
            if (SelectedSpot == null)
                return;

            await ExecuteAsync(async () =>
            {
                SelectedSpot.Rating = rating;
                await _databaseService.SaveAstroSpotAsync(SelectedSpot);
            }, "Failed to update rating");
        }

        [RelayCommand]
        private async Task NavigateToSpotAsync(AstroSpot spot)
        {
            if (spot == null)
                return;

            await ExecuteAsync(async () =>
            {
                // Generate Google Maps URL
                string url = spot.GetGoogleMapsUrl();

                // Open in external navigation app
                await Launcher.OpenAsync(new Uri(url));
            }, "Failed to open navigation");
        }

        [RelayCommand]
        private async Task ViewSpotDetailAsync(AstroSpot spot)
        {
            if (spot == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(SpotDetailPage)}?spotId={spot.Id}");
        }

        private void InitializeMap()
        {
            _map = new Map();

            // Add base layer (OpenStreetMap)
            _map.Layers.Add(OpenStreetMap.CreateTileLayer());

            // Initialize the map position to a default location (will be updated later)
            _map.Navigator.CenterOn(SphericalMercator.FromLonLat(0, 0).ToMPoint());
            _map.Navigator.ZoomTo(2);
        }

        private void UpdateLightPollutionLayer()
        {
            // Find and remove existing light pollution layer if it exists
            var existingLayer = _map.Layers.FirstOrDefault(l => l.Name == "LightPollution");
            if (existingLayer != null)
            {
                _map.Layers.Remove(existingLayer);
            }

            if (IsLightPollutionLayerVisible)
            {
                // Add light pollution layer (using Light Pollution Map tile service)
                var lightPollutionLayer = new TileLayer(
                    provider: new HttpTileSource(
                    urlFormatter: "https://tiles.lightpollutionmap.info/tiles/world/tile_{z}_{x}_{y}.png",
                    name: "LightPollution",
                    attribution: "Light Pollution Map")
                    {
                        MaxZoomLevel = 16,
                        MinZoomLevel = 1
                    })
                {
                    Name = "LightPollution",
                    Opacity = 0.7f
                };

                _map.Layers.Add(lightPollutionLayer);
            }
        }

        private async Task UpdateMapPins()
        {
            // Find and remove existing pins layer if it exists
            var existingLayer = _map.Layers.FirstOrDefault(l => l.Name == "Pins");
            if (existingLayer != null)
            {
                _map.Layers.Remove(existingLayer);
            }

            // Create a new memory provider for the pins
            var pinLayer = new MemoryLayer();
            pinLayer.Name = "Pins";

            // Add pins for all astro spots
            foreach (var spot in AstroSpots)
            {
                var feature = new Mapsui.Features.Feature
                {
                    Geometry = SphericalMercator.FromLonLat(spot.Longitude, spot.Latitude).ToMPoint(),
                    ["Id"] = spot.Id,
                    ["Name"] = spot.Name,
                    ["Rating"] = spot.Rating
                };

                pinLayer.Features.Add(feature);
            }

            // Add the layer to the map
            _map.Layers.Add(pinLayer);
        }

        // Call this when the page appears
        public async Task InitializeAsync()
        {
            await LoadDataAsync();
            await GetCurrentLocationAsync();
        }
    }
}
