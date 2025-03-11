
using System;
using System.Collections.Generic;

namespace AstroToolkitWeb.Models
{
    public class Forecast
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string LocationName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<WeatherData> HourlyForecasts { get; set; } = new List<WeatherData>();
        public WeatherData CurrentWeather { get; set; }
    }
}
