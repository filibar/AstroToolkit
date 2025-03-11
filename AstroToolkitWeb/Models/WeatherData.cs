
using System;
using System.ComponentModel.DataAnnotations;

namespace AstroToolkitWeb.Models
{
    public class WeatherData
    {
        // Primary key
        public int Id { get; set; }
        
        // Location information
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? LocationName { get; set; }
        
        // Date and time
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Forecast time for predictions
        public DateTime ForecastTime { get; set; } = DateTime.UtcNow;
        
        // Temperature in Celsius
        public double? Temperature { get; set; }
        
        // Feels like temperature in Celsius
        public double? FeelsLike { get; set; }
        
        // Humidity percentage (0-100)
        [Range(0, 100)]
        public double? Humidity { get; set; }
        
        // Wind speed in m/s
        public double? WindSpeed { get; set; }
        
        // Wind direction in degrees (0-360)
        [Range(0, 360)]
        public double? WindDirection { get; set; }
        
        // Cloud coverage percentage (0-100)
        [Range(0, 100)]
        public double? CloudCoverage { get; set; }
        
        // Precipitation amount in mm
        public double? Precipitation { get; set; }
        
        // Probability of precipitation (0-100)
        [Range(0, 100)]
        public double? PrecipitationProbability { get; set; }
        
        // Atmospheric pressure in hPa
        public double? Pressure { get; set; }
        
        // Weather condition code (OpenWeatherMap)
        public int? WeatherCode { get; set; }
        
        // Weather icon code
        public string? IconCode { get; set; }
        
        // Weather description (clear, cloudy, etc.)
        public string? WeatherDescription { get; set; }
        
        // UV index (0-11+)
        [Range(0, 15)]
        public double? UvIndex { get; set; }
        
        // Astronomy viewing score (0-100)
        [Range(0, 100)]
        public int? AstronomyViewingScore { get; set; }
        
        // Astronomy rating (1-10)
        [Range(1, 10)]
        public int? AstroRating { get; set; }
        
        // Seeing conditions rated 1-10 (1 = poor, 10 = excellent)
        [Range(1, 10)]
        public int? SeeingIndex { get; set; }
        
        // Transparency rated 1-10 (1 = poor, 10 = excellent)
        [Range(1, 10)]
        public int? TransparencyIndex { get; set; }
        
        // Weather stability rating (1-10)
        [Range(1, 10)]
        public int? StabilityIndex { get; set; }
        
        // Light pollution level (Bortle scale 1-9)
        [Range(1, 9)]
        public int? LightPollutionIndex { get; set; }
        
        // Sunrise and Sunset times
        public DateTime? SunriseTime { get; set; }
        public DateTime? SunsetTime { get; set; }
        
        // Moon illumination percentage (0-100)
        [Range(0, 100)]
        public double? MoonIllumination { get; set; }
        
        // Moon phase information (object with detailed moon phase data)
        public MoonPhase? MoonPhase { get; set; }
        
        // Moon rise time
        public DateTime? MoonRise { get; set; }
        
        // Moon set time
        public DateTime? MoonSet { get; set; }
    }
}
