namespace AstroToolkit.Views
{
    public partial class SpotDetailPage : ContentPage
    {
        private readonly SpotDetailViewModel _viewModel;

        public SpotDetailPage(SpotDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateRatingStars();
            
            // Attach property changed event to update UI elements
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Detach event to prevent memory leaks
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.Spot) || e.PropertyName == nameof(_viewModel.IsEditing))
            {
                UpdateRatingStars();
            }
        }

        private void UpdateRatingStars()
        {
            // Clear existing stars
            RatingStars.Children.Clear();
            
            if (_viewModel.Spot == null)
                return;

            for (int i = 1; i <= 5; i++)
            {
                var starButton = new Button
                {
                    Text = i <= _viewModel.Spot.Rating ? "★" : "☆",
                    FontSize = 20,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    Padding = 0,
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Yellow,
                    BorderWidth = 0
                };
                
                if (_viewModel.IsEditing)
                {
                    int rating = i; // Capture the value for use in lambda
                    starButton.Clicked += (s, e) => _viewModel.UpdateRatingCommand.Execute(rating);
                }
                
                RatingStars.Children.Add(starButton);
            }
        }
    }
}
