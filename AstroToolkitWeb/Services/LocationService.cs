using AstroToolkitWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AstroToolkitWeb.Services
{
    public class LocationService
    {
        private readonly DatabaseService _dbService;
        private readonly ILogger<LocationService> _logger;
        private readonly IConfiguration _configuration;

        public LocationService(
            DatabaseService dbService,
            ILogger<LocationService> logger,
            IConfiguration configuration)
        {
            _dbService = dbService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<AstroSpot>> GetAllSpotsAsync()
        {
            try
            {
                var spots = await _dbService.GetAllSpotsAsync();
                return spots;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all spots");
                return new List<AstroSpot>();
            }
        }

        public async Task<AstroSpot?> GetSpotByIdAsync(int id)
        {
            try
            {
                return await _dbService.GetSpotByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving spot with ID {SpotId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<AstroSpot>> GetSpotsByUserIdAsync(string userId)
        {
            try
            {
                var spots = await _dbService.GetSpotsByUserIdAsync(userId);
                return spots;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving spots for user {UserId}", userId);
                return new List<AstroSpot>();
            }
        }

        public async Task<AstroSpot?> AddSpotAsync(AstroSpot spot)
        {
            try
            {
                // Calculate light pollution level (would typically use a real API)
                // This is a simplified example
                if (spot.LightPollutionLevel <= 0)
                {
                    // Default to medium light pollution if not specified
                    spot.LightPollutionLevel = 5;
                }

                // Set creation date
                spot.CreatedDate = DateTime.UtcNow;

                return await _dbService.AddSpotAsync(spot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding spot");
                return null;
            }
        }

        public async Task<double?> GetElevationAsync(double latitude, double longitude)
        {
            try
            {
                // In a real application, we would call an elevation API here
                // For now, return a random value between 0 and 1000 meters
                await Task.Delay(100); // Simulate API call
                Random rnd = new Random();
                return rnd.Next(0, 1000);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting elevation data");
                return null;
            }
        }

        public int CalculateLightPollutionLevel(double latitude, double longitude)
        {
            // In a real application, this would call a light pollution API or database
            // For demonstration, calculate a value based on latitude/longitude
            // This is just a placeholder and does not reflect actual light pollution

            Random rnd = new Random(
                (int)(latitude * 1000) + 
                (int)(longitude * 1000));

            // Generate a Bortle scale value (1-9) with higher probability for middle values
            int[] weights = { 5, 10, 15, 25, 20, 10, 8, 5, 2 };
            int sum = weights.Sum();
            int randomValue = rnd.Next(sum);
            
            int currentSum = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                currentSum += weights[i];
                if (randomValue < currentSum)
                {
                    return i + 1; // Bortle scale is 1-9
                }
            }
            
            return 5; // Default to medium light pollution
        }
    }
}