using Mapsui.UI.Maui;
using Microsoft.Maui.Controls;

namespace AstroToolkit.Views
{
    public partial class MapPage : ContentPage
    {
        private readonly MapViewModel _viewModel;

        public MapPage(MapViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            
            // Initialize map control
            mapView.Map = _viewModel.MapInstance;
            
            // Setup map interaction events
            SetupMapInteraction();
            
            // Setup Bortle scale options
            _viewModel.BortleScaleOptions = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
            RenderRatingStars();
        }

        private void SetupMapInteraction()
        {
            mapView.MapClicked += OnMapClicked;
            mapView.MapLongClicked += OnMapLongClicked;
            mapView.FeatureClicked += OnFeatureClicked;
        }

        private void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            if (_viewModel.IsAddingNewSpot)
            {
                // Update the new spot location
                if (_viewModel.SelectedSpot != null)
                {
                    var coordinates = Mapsui.Projections.SphericalMercator.ToLonLat(e.Point.X, e.Point.Y);
                    _viewModel.SelectedSpot.Longitude = coordinates.X;
                    _viewModel.SelectedSpot.Latitude = coordinates.Y;
                }
            }
            else
            {
                // Deselect spot if clicked outside
                _viewModel.SelectedSpot = null;
            }
        }

        private void OnMapLongClicked(object sender, MapLongClickedEventArgs e)
        {
            if (!_viewModel.IsAddingNewSpot)
            {
                var coordinates = Mapsui.Projections.SphericalMercator.ToLonLat(e.Point.X, e.Point.Y);
                
                // Create a new spot at the long-pressed location
                _viewModel.SelectedSpot = new AstroSpot
                {
                    Latitude = coordinates.Y,
                    Longitude = coordinates.X,
                    CreatedDate = DateTime.Now,
                    Name = "New Spot",
                    Rating = 0
                };
                
                _viewModel.IsAddingNewSpot = true;
            }
        }

        private async void OnFeatureClicked(object sender, FeatureClickedEventArgs e)
        {
            if (_viewModel.IsAddingNewSpot)
                return;

            if (e.Feature != null && e.Feature.TryGetValue("Id", out var spotId))
            {
                var spot = await _viewModel._databaseService.GetAstroSpotAsync(Convert.ToInt32(spotId));
                if (spot != null)
                {
                    _viewModel.SelectedSpot = spot;
                    RenderRatingStars();
                }
            }
        }

        private void RenderRatingStars()
        {
            // Clear existing stars
            RatingStars.Children.Clear();
            
            if (_viewModel.SelectedSpot == null)
                return;

            for (int i = 1; i <= 5; i++)
            {
                var starButton = new Button
                {
                    Text = i <= _viewModel.SelectedSpot.Rating ? "★" : "☆",
                    FontSize = 20,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    Padding = 0,
                    BackgroundColor = Colors.Transparent
                };
                
                int rating = i; // Capture the value for use in lambda
                starButton.Clicked += (s, e) => _viewModel.RateSpotCommand.Execute(rating);
                
                RatingStars.Children.Add(starButton);
            }
        }
    }
}
