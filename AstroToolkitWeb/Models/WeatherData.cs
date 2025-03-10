using System.ComponentModel.DataAnnotations;

namespace AstroToolkitWeb.Models
{
    public class WeatherData
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        public string? LocationName { get; set; }

        // Temperature in Celsius
        public double? Temperature { get; set; }

        // Cloud coverage percentage (0-100)
        [Range(0, 100)]
        public int CloudCoverage { get; set; }

        // Humidity percentage (0-100)
        [Range(0, 100)]
        public int Humidity { get; set; }

        // Wind speed in m/s
        public double? WindSpeed { get; set; }

        // Wind direction in degrees (0-360)
        [Range(0, 360)]
        public int? WindDirection { get; set; }

        // Precipitation probability (0-100)
        [Range(0, 100)]
        public int PrecipitationProbability { get; set; }
        
        // Precipitation amount in mm
        public double? Precipitation { get; set; }

        // Seeing conditions rated 1-10 (1 = poor, 10 = excellent)
        [Range(1, 10)]
        public int? SeeingIndex { get; set; }

        // Transparency rated 1-10 (1 = poor, 10 = excellent)
        [Range(1, 10)]
        public int? TransparencyIndex { get; set; }

        // Overall rating for astrophotography (1-10)
        [Range(1, 10)]
        public int? AstroRating { get; set; }
        
        // Astronomy viewing score (0-100)
        [Range(0, 100)]
        public int AstronomyViewingScore { get; set; } = 0;

        public DateTime ForecastTime { get; set; } = DateTime.UtcNow;
        
        public DateTime? SunriseTime { get; set; }
        
        public DateTime? SunsetTime { get; set; }

        public string? WeatherDescription { get; set; }

        public string? IconCode { get; set; }

        public MoonPhase? MoonPhase { get; set; }
    }
}