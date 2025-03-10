using Microsoft.Maui.Devices.Sensors;

namespace AstroToolkit.Services
{
    public class LocationService
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                    return location;
                }
            }
            catch (FeatureNotSupportedException)
            {
                // Handle not supported on device exception
                await Application.Current.MainPage.DisplayAlert("Error", "Location services are not supported on this device.", "OK");
            }
            catch (FeatureNotEnabledException)
            {
                // Handle not enabled on device exception
                await Application.Current.MainPage.DisplayAlert("Error", "Location services are not enabled on this device.", "OK");
            }
            catch (PermissionException)
            {
                // Handle permission exception
                await Application.Current.MainPage.DisplayAlert("Error", "Location permission not granted.", "OK");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
            }
            finally
            {
                _isCheckingLocation = false;
            }

            return null;
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
                _cancelTokenSource.Cancel();
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                await Application.Current.MainPage.DisplayAlert("Location Denied", 
                    "Please enable location access in your app settings to use this feature.", "OK");
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                // Prompt the user with additional information as to why the permission is needed
                await Application.Current.MainPage.DisplayAlert("Location Permission", 
                    "We need access to your location to show your current position on the map and provide accurate astronomical calculations.", "OK");
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }

        public bool IsLocationAvailable()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
        }
    }
}
