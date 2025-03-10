namespace AstroToolkit.Models
{
    public class WeatherData
    {
        // Basic weather information
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public double Pressure { get; set; }
        public double Visibility { get; set; }
        public string WeatherDescription { get; set; }
        public string WeatherIcon { get; set; }
        
        // Cloud coverage information
        public int CloudCoverageTotal { get; set; }
        public int CloudCoverageLow { get; set; }
        public int CloudCoverageMid { get; set; }
        public int CloudCoverageHigh { get; set; }
        
        // Astronomical information
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public double MoonIllumination { get; set; } // 0.0 to 1.0
        public string MoonPhase { get; set; }
        public int DaysToNewMoon { get; set; }
        public int DaysToFullMoon { get; set; }
        
        // Air quality information relevant for visibility
        public int AirQualityIndex { get; set; }
        
        // Forecast timestamp
        public DateTime Timestamp { get; set; }
        
        // Location information
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; }
        
        // Astrophotography-specific metrics
        public bool IsGoodForAstrophotography => 
            CloudCoverageTotal < 20 && 
            Visibility > 8 && 
            MoonIllumination < 0.3 && 
            Humidity < 80;

        // Calculate seeing conditions based on atmospheric stability
        public string SeeingConditions
        {
            get
            {
                if (WindSpeed < 5 && CloudCoverageTotal < 10 && Humidity < 70)
                    return "Excellent";
                else if (WindSpeed < 10 && CloudCoverageTotal < 20 && Humidity < 80)
                    return "Good";
                else if (WindSpeed < 15 && CloudCoverageTotal < 40)
                    return "Fair";
                else
                    return "Poor";
            }
        }
    }

    public class WeatherForecast
    {
        public List<WeatherData> HourlyForecast { get; set; } = new List<WeatherData>();
        public List<WeatherData> DailyForecast { get; set; } = new List<WeatherData>();
    }
}
