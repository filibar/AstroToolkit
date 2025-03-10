namespace AstroToolkit.Views
{
    public partial class WeatherPage : ContentPage
    {
        private readonly WeatherViewModel _viewModel;

        public WeatherPage(WeatherViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}
