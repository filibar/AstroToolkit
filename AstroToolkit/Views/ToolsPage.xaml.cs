namespace AstroToolkit.Views
{
    public partial class ToolsPage : ContentPage
    {
        private readonly ToolsViewModel _viewModel;

        public ToolsPage(ToolsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            
            // Position the Polaris marker based on the calculated angle and offset
            UpdatePolarisMarker();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
            UpdatePolarisMarker();
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            // Update calculations when sliders change
            _viewModel.UpdateMaxExposureTimeCommand.Execute(null);
        }

        private void UpdatePolarisMarker()
        {
            // Convert angle and offset to x,y coordinates within the polar scope circle
            double angle = _viewModel.PolarisAngle * Math.PI / 180; // Convert to radians
            double offset = _viewModel.PolarisOffset;
            
            // Calculate radius as percentage of the total circle radius (90px)
            // Normalize offset to be within the circle (assumes offset is in degrees from pole)
            double radius = (offset / 10) * 90; // Scale to fit within the circle
            
            // Calculate x,y position (center of circle is at 90,90)
            double x = 90 + radius * Math.Sin(angle);
            double y = 90 - radius * Math.Cos(angle);
            
            // Position the Polaris marker (center of the 10px width/height ellipse, so we adjust)
            PolarisMarker.TranslationX = x - 90 - 5; // Adjust for the 10px width (5px radius)
            PolarisMarker.TranslationY = y - 90 - 5; // Adjust for the 10px height (5px radius)
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            
            // Update the Polaris marker when the angle or offset changes
            if (propertyName == nameof(_viewModel.PolarisAngle) || 
                propertyName == nameof(_viewModel.PolarisOffset))
            {
                UpdatePolarisMarker();
            }
        }
    }
}
