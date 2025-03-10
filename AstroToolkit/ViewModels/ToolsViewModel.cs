using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AstroToolkit.ViewModels
{
    public partial class ToolsViewModel : BaseViewModel
    {
        private readonly LocationService _locationService;
        private readonly AstroCalculationService _astroCalculationService;

        [ObservableProperty]
        private double _currentLatitude;

        [ObservableProperty]
        private double _currentLongitude;

        [ObservableProperty]
        private string _formattedLatitude;

        [ObservableProperty]
        private string _formattedLongitude;

        // 500 Rule Calculator properties
        [ObservableProperty]
        private double _focalLength = 50;

        [ObservableProperty]
        private double _cropFactor = 1.0;

        [ObservableProperty]
        private double _maxExposureTime;

        // NPF Rule Calculator properties
        [ObservableProperty]
        private double _aperture = 2.8;

        [ObservableProperty]
        private double _pixelPitch = 4.0;

        [ObservableProperty]
        private double _npfExposureTime;

        // Sensor properties
        [ObservableProperty]
        private double _sensorWidth = 36;

        [ObservableProperty]
        private double _sensorHeight = 24;

        [ObservableProperty]
        private double _calculatedCropFactor;
        
        [ObservableProperty]
        private double _fieldOfView;

        // Polar alignment properties
        [ObservableProperty]
        private double _polarisAngle;

        [ObservableProperty]
        private double _polarisOffset;

        // Sensor types for quick selection
        [ObservableProperty]
        private ObservableCollection<SensorType> _sensorTypes;

        public ToolsViewModel(LocationService locationService, AstroCalculationService astroCalculationService)
        {
            Title = "Astro Tools";
            _locationService = locationService;
            _astroCalculationService = astroCalculationService;
            
            // Initialize sensor types
            SensorTypes = new ObservableCollection<SensorType>
            {
                new SensorType { Name = "Full Frame (35mm)", Width = 36, Height = 24, CropFactor = 1.0 },
                new SensorType { Name = "APS-C (Canon)", Width = 22.3, Height = 14.9, CropFactor = 1.6 },
                new SensorType { Name = "APS-C (Nikon/Sony)", Width = 23.6, Height = 15.6, CropFactor = 1.5 },
                new SensorType { Name = "Micro 4/3", Width = 17.3, Height = 13, CropFactor = 2.0 },
                new SensorType { Name = "1-inch", Width = 13.2, Height = 8.8, CropFactor = 2.7 }
            };
            
            // Initial calculations
            UpdateMaxExposureTime();
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
                    
                    // Format for display
                    FormattedLatitude = _astroCalculationService.DegreesToDMS(CurrentLatitude, true);
                    FormattedLongitude = _astroCalculationService.DegreesToDMS(CurrentLongitude, false);
                    
                    // Update polar alignment calculations
                    UpdatePolarAlignment();
                }
                else
                {
                    throw new Exception("Could not get current location.");
                }
            }, "Failed to get current location");
        }

        [RelayCommand]
        private void UpdateMaxExposureTime()
        {
            MaxExposureTime = _astroCalculationService.Calculate500Rule(FocalLength, CropFactor);
            NpfExposureTime = _astroCalculationService.CalculateNpfRule(FocalLength, Aperture, PixelPitch);
            FieldOfView = _astroCalculationService.CalculateFieldOfView(FocalLength, SensorWidth);
        }

        [RelayCommand]
        private void UpdatePolarAlignment()
        {
            var polarisPosition = _astroCalculationService.CalculatePolarisPosition(
                DateTime.Now, CurrentLatitude, CurrentLongitude);
                
            PolarisAngle = polarisPosition.Angle;
            PolarisOffset = polarisPosition.Offset;
        }

        [RelayCommand]
        private void SelectSensorType(SensorType sensorType)
        {
            if (sensorType != null)
            {
                SensorWidth = sensorType.Width;
                SensorHeight = sensorType.Height;
                CropFactor = sensorType.CropFactor;
                
                // Update calculations
                UpdateMaxExposureTime();
            }
        }

        [RelayCommand]
        private void CalculateCropFactor()
        {
            CalculatedCropFactor = _astroCalculationService.CalculateCropFactor(SensorWidth, SensorHeight);
            CropFactor = CalculatedCropFactor; // Update the crop factor used in calculations
            
            // Update other calculations
            UpdateMaxExposureTime();
        }

        // Call this when the page appears
        public async Task InitializeAsync()
        {
            await GetCurrentLocationAsync();
            UpdateMaxExposureTime();
        }
    }

    public class SensorType
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double CropFactor { get; set; }
    }
}
