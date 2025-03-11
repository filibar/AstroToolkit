
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

        // Astronomy viewing score (0-100)
        [Range(0, 100)]
        public int? AstronomyViewingScore { get; set; }

        // Seeing conditions rated 1-10 (1 = poor, 10 = excellent)
        [Range(1, 10)]
        public int? SeeingIndex { get; set; }
        
        // Weather condition (clear, cloudy, etc.)
        public string? WeatherCondition { get; set; }
        
        // Icon code for weather API
        public string? IconCode { get; set; }

        // Transparency rated 1-10 (1 = poor, 10 = excellent)
        [Range(1, 10)]
        public int? TransparencyIndex { get; set; }
        
        // Weather stability rating (1-10)
        [Range(1, 10)]
        public int? StabilityIndex { get; set; }
        
        // Light pollution level (Bortle scale 1-9)
        [Range(1, 9)]
        public int? LightPollutionIndex { get; set; }
        
        // Sunrise time
        public DateTime? Sunrise { get; set; }
        
        // Sunset time
        public DateTime? Sunset { get; set; }
        
        // Moon illumination percentage (0-100)
        [Range(0, 100)]
        public double? MoonIllumination { get; set; }
        
        // Moon phase (0-1 for new to full)
        [Range(0, 1)]
        public double? MoonPhase { get; set; }
        
        // Moon rise time
        public DateTime? MoonRise { get; set; }
        
        // Moon set time
        public DateTime? MoonSet { get; set; }
    }
}
