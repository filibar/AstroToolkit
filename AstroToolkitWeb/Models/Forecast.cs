
using System;
using System.ComponentModel.DataAnnotations;

namespace AstroToolkitWeb.Models
{
    public class Forecast
    {
        public int Id { get; set; }
        
        public DateTime Date { get; set; }
        
        public string? Description { get; set; }
        
        public double? Temperature { get; set; }
        
        public double? CloudCoverage { get; set; }
        
        public double? Humidity { get; set; }
        
        public double? WindSpeed { get; set; }
        
        public string? WindDirection { get; set; }
        
        public double? ChanceOfRain { get; set; }
        
        public double? AirQualityIndex { get; set; }
        
        public string? IconCode { get; set; }
        
        // Returns the appropriate icon path for this forecast
        public string GetIconPath()
        {
            return !string.IsNullOrEmpty(IconCode)
                ? $"_content/AstroToolkitWeb/images/weather/{IconCode}.svg"
                : "_content/AstroToolkitWeb/images/weather/placeholder.svg";
        }
    }
}
