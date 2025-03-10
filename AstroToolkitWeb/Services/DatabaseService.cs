using Microsoft.EntityFrameworkCore;
using AstroToolkitWeb.Data;
using AstroToolkitWeb.Models;

namespace AstroToolkitWeb.Services
{
    public class DatabaseService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(ApplicationDbContext dbContext, ILogger<DatabaseService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<AstroSpot>> GetAllSpotsAsync()
        {
            try
            {
                return await _dbContext.AstroSpots
                    .Include(a => a.Photos)
                    .ToListAsync();
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
                return await _dbContext.AstroSpots
                    .Include(a => a.Photos)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving spot with ID {SpotId}", id);
                return null;
            }
        }

        public async Task<List<AstroSpot>> GetSpotsByUserIdAsync(string userId)
        {
            try
            {
                return await _dbContext.AstroSpots
                    .Include(a => a.Photos)
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
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
                _dbContext.AstroSpots.Add(spot);
                await _dbContext.SaveChangesAsync();
                return spot;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding spot");
                return null;
            }
        }

        public async Task<WeatherData?> GetWeatherDataForLocationAsync(double latitude, double longitude, DateTime date)
        {
            try
            {
                return await _dbContext.WeatherData
                    .Where(w => w.Latitude == latitude && w.Longitude == longitude && w.Date.Date == date.Date)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather data");
                return null;
            }
        }

        public async Task<WeatherData?> AddWeatherDataAsync(WeatherData weatherData)
        {
            try
            {
                // Check if entry already exists for this location and date
                var existingData = await _dbContext.WeatherData
                    .Where(w => w.Latitude == weatherData.Latitude && 
                           w.Longitude == weatherData.Longitude && 
                           w.Date.Date == weatherData.Date.Date)
                    .FirstOrDefaultAsync();

                if (existingData != null)
                {
                    // Update existing entry
                    _dbContext.Entry(existingData).CurrentValues.SetValues(weatherData);
                }
                else
                {
                    // Add new entry
                    _dbContext.WeatherData.Add(weatherData);
                }

                await _dbContext.SaveChangesAsync();
                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving weather data");
                return null;
            }
        }
    }
}