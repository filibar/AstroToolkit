using System.Net.Http.Json;
using AstroToolkitWeb.Models;

namespace AstroToolkitWeb.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DatabaseService _dbService;
        private readonly AstroCalculationService _astroCalculationService;
        private readonly string? _apiKey;

        public WeatherService(
            HttpClient httpClient,
            ILogger<WeatherService> logger,
            IConfiguration configuration,
            DatabaseService dbService,
            AstroCalculationService astroCalculationService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _dbService = dbService;
            _astroCalculationService = astroCalculationService;
            
            // Get the API key from configuration
            _apiKey = _configuration["WeatherApi:ApiKey"];
        }

        /// <summary>
        /// Gets the current weather data for a specific location
        /// </summary>
        public async Task<WeatherData?> GetWeatherDataAsync(double latitude, double longitude, DateTime? date = null)
        {
            try
            {
                // If no date is provided, use current date
                date ??= DateTime.UtcNow;

                // Check if we already have weather data for this location and date in the database
                var existingData = await _dbService.GetWeatherDataForLocationAsync(latitude, longitude, date.Value);
                
                if (existingData != null)
                {
                    // If the data is recent (less than 3 hours old), return it
                    if (existingData.ForecastTime.AddHours(3) > DateTime.UtcNow)
                    {
                        return existingData;
                    }
                }

                // For demonstration purposes, create simulated weather data
                // In a real app, this would call a weather API
                if (string.IsNullOrEmpty(_apiKey))
                {
                    _logger.LogWarning("Weather API key not configured. Using simulated data.");
                    return CreateSimulatedWeatherData(latitude, longitude, date.Value);
                }

                // Call weather API (placeholder for now - would need to be replaced with actual API call)
                // var weatherApiUrl = $"https://api.example.com/weather?lat={latitude}&lon={longitude}&appid={_apiKey}";
                // var response = await _httpClient.GetFromJsonAsync<WeatherApiResponse>(weatherApiUrl);
                
                // For demo purposes, use simulated data
                var weatherData = CreateSimulatedWeatherData(latitude, longitude, date.Value);
                
                // Store in database
                await _dbService.AddWeatherDataAsync(weatherData);
                
                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather data for location {Latitude}, {Longitude}", latitude, longitude);
                return null;
            }
        }

        /// <summary>
        /// Gets an astronomy viewing score (0-100) for the given location and date
        /// </summary>
        public async Task<int> GetAstronomyViewingScoreAsync(double latitude, double longitude, DateTime date)
        {
            try
            {
                var weatherData = await GetWeatherDataAsync(latitude, longitude, date);
                
                if (weatherData == null)
                {
                    return 0;  // No data available
                }

                // Calculate astronomy viewing score based on various factors
                // This is a simple algorithm for demonstration purposes
                
                // Cloud cover has the biggest impact (0-60 points)
                int cloudScore = 60 - (int)(weatherData.CloudCoverage * 0.6);
                
                // Humidity affects visibility (0-15 points)
                int humidityScore = 15 - (int)(weatherData.Humidity * 0.15);
                
                // Temperature stability is important (up to 10 points)
                // Moderate temps are better (not too hot or cold)
                double tempCelsius = weatherData.Temperature ?? 15;
                int tempScore = tempCelsius < 0 ? 5 : (tempCelsius > 30 ? 5 : 10);
                
                // Wind affects image stability (0-15 points)
                double windSpeed = weatherData.WindSpeed ?? 0;
                int windScore = windSpeed > 20 ? 0 : (windSpeed > 10 ? 5 : 15);
                
                // Calculate total score (0-100)
                int totalScore = cloudScore + humidityScore + tempScore + windScore;
                
                // Ensure score is between 0 and 100
                totalScore = Math.Max(0, Math.Min(100, totalScore));
                
                // Update the weather data with the astronomy viewing score
                weatherData.AstronomyViewingScore = totalScore;
                await _dbService.AddWeatherDataAsync(weatherData);
                
                return totalScore;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating astronomy viewing score");
                return 0;
            }
        }

        /// <summary>
        /// Creates simulated weather data for demonstration purposes
        /// </summary>
        private WeatherData CreateSimulatedWeatherData(double latitude, double longitude, DateTime date)
        {
            // Generate random but plausible weather data
            Random random = new Random(
                (int)(latitude * 1000) + 
                (int)(longitude * 1000) + 
                date.Day + date.Month + date.Year);
            
            // Calculate moon phase
            var moonPhase = _astroCalculationService.CalculateMoonPhase(date);
            
            // Base temperature on latitude (colder near poles, warmer near equator)
            double baseTemp = 15 - Math.Abs(latitude) * 0.4;
            
            // Add seasonal variation (northern/southern hemisphere)
            int month = date.Month;
            bool northernHemisphere = latitude >= 0;
            
            // For northern hemisphere: summer in Jun-Aug, winter in Dec-Feb
            // For southern hemisphere: summer in Dec-Feb, winter in Jun-Aug
            double seasonalOffset;
            if (northernHemisphere)
            {
                seasonalOffset = Math.Sin((month - 1) * Math.PI / 6) * 10;
            }
            else
            {
                seasonalOffset = Math.Sin((month - 7) * Math.PI / 6) * 10;
            }
            
            double temperature = baseTemp + seasonalOffset + random.Next(-5, 6);
            
            // Cloud cover tends to be higher in certain seasons
            int cloudBase = Math.Abs(month - 6) < 3 ? 30 : 50;
            int cloudCoverage = Math.Min(100, Math.Max(0, cloudBase + random.Next(-20, 21)));
            
            // Generate weather description based on cloud coverage
            string weatherDescription;
            if (cloudCoverage < 20)
            {
                weatherDescription = "Clear Sky";
            }
            else if (cloudCoverage < 50)
            {
                weatherDescription = "Partly Cloudy";
            }
            else if (cloudCoverage < 80)
            {
                weatherDescription = "Mostly Cloudy";
            }
            else
            {
                weatherDescription = "Overcast";
            }
            
            // Generate simulated wind speed (m/s)
            double windSpeed = random.Next(0, 15) + random.NextDouble();
            
            // Generate simulated wind direction (degrees)
            int windDirection = random.Next(0, 360);
            
            // Generate simulated humidity (%)
            int humidity = random.Next(30, 95);
            
            // Generate precipitation probability (higher with more clouds)
            int precipProbability = Math.Min(100, cloudCoverage + random.Next(-20, 20));
            
            // Generate precipitation amount (mm)
            double precipitation = precipProbability > 50 ? random.NextDouble() * 8 : 0;
            
            // Seeing index (astronomical term for atmospheric steadiness, 1-10)
            // Higher wind speeds reduce seeing quality
            int seeingIndex = Math.Max(1, Math.Min(10, 10 - (int)(windSpeed / 2) - random.Next(0, 3)));
            
            // Transparency index (1-10)
            // Higher humidity reduces transparency
            int transparencyIndex = Math.Max(1, Math.Min(10, 10 - (humidity / 10) - (cloudCoverage / 20)));
            
            // Calculate overall astro rating (1-10)
            int astroRating = Math.Max(1, (seeingIndex + transparencyIndex + (10 - cloudCoverage / 10)) / 3);
            
            // Calculate sunrise/sunset times (approximation)
            DateTime baseDate = date.Date;
            double dayLengthHours = 12 + Math.Cos(latitude * Math.PI / 180) * Math.Cos((month - 6) * Math.PI / 6) * 6;
            
            TimeSpan halfDayLength = TimeSpan.FromHours(dayLengthHours / 2);
            DateTime noonTime = baseDate.AddHours(12);
            DateTime sunriseTime = noonTime - halfDayLength;
            DateTime sunsetTime = noonTime + halfDayLength;
            
            // Adjust for random variations
            sunriseTime = sunriseTime.AddMinutes(random.Next(-15, 16));
            sunsetTime = sunsetTime.AddMinutes(random.Next(-15, 16));
            
            return new WeatherData
            {
                Date = date.Date,
                Latitude = latitude,
                Longitude = longitude,
                LocationName = $"Location at {latitude:F4}, {longitude:F4}",
                Temperature = Math.Round(temperature, 1),
                CloudCoverage = cloudCoverage,
                Humidity = humidity,
                WindSpeed = Math.Round(windSpeed, 1),
                WindDirection = windDirection,
                PrecipitationProbability = precipProbability,
                Precipitation = Math.Round(precipitation, 1),
                SeeingIndex = seeingIndex,
                TransparencyIndex = transparencyIndex,
                AstroRating = astroRating,
                ForecastTime = DateTime.UtcNow,
                WeatherDescription = weatherDescription,
                IconCode = GetWeatherIconCode(cloudCoverage),
                MoonPhase = moonPhase,
                SunriseTime = sunriseTime,
                SunsetTime = sunsetTime
            };
        }

        /// <summary>
        /// Maps cloud coverage to a weather icon code
        /// </summary>
        private string GetWeatherIconCode(int cloudCoverage)
        {
            if (cloudCoverage < 20)
            {
                return "clear";
            }
            else if (cloudCoverage < 50)
            {
                return "partly-cloudy";
            }
            else if (cloudCoverage < 80)
            {
                return "cloudy";
            }
            else
            {
                return "overcast";
            }
        }
    }
}