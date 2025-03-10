using RestSharp;
using System.Text.Json;

namespace AstroToolkit.Services
{
    public class WeatherService
    {
        private readonly RestClient _client;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

        public WeatherService()
        {
            _client = new RestClient(BaseUrl);
            _apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY") ?? "";
        }

        public async Task<WeatherData> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("Weather API key is missing. Please set the WEATHER_API_KEY environment variable.");
            }

            try
            {
                var request = new RestRequest("weather", Method.Get);
                request.AddParameter("lat", latitude);
                request.AddParameter("lon", longitude);
                request.AddParameter("appid", _apiKey);
                request.AddParameter("units", "metric");

                var response = await _client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    throw new Exception($"Error fetching weather data: {response.ErrorMessage}");
                }

                var weatherResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(response.Content);
                return MapToWeatherData(weatherResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCurrentWeatherAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<MoonPhase> GetMoonPhaseAsync(double latitude, double longitude, DateTime? date = null)
        {
            if (date == null)
                date = DateTime.Today;

            long timestamp = new DateTimeOffset(date.Value).ToUnixTimeSeconds();

            try
            {
                var request = new RestRequest("onecall", Method.Get);
                request.AddParameter("lat", latitude);
                request.AddParameter("lon", longitude);
                request.AddParameter("appid", _apiKey);
                request.AddParameter("exclude", "current,minutely,hourly,alerts");
                request.AddParameter("units", "metric");

                var response = await _client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    throw new Exception($"Error fetching moon data: {response.ErrorMessage}");
                }

                var oneCallResponse = JsonSerializer.Deserialize<OneCallResponse>(response.Content);
                return ExtractMoonPhase(oneCallResponse, date.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMoonPhaseAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<WeatherForecast> GetWeatherForecastAsync(double latitude, double longitude)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("Weather API key is missing. Please set the WEATHER_API_KEY environment variable.");
            }

            try
            {
                var request = new RestRequest("onecall", Method.Get);
                request.AddParameter("lat", latitude);
                request.AddParameter("lon", longitude);
                request.AddParameter("appid", _apiKey);
                request.AddParameter("exclude", "current,minutely,alerts");
                request.AddParameter("units", "metric");

                var response = await _client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    throw new Exception($"Error fetching forecast data: {response.ErrorMessage}");
                }

                var forecastResponse = JsonSerializer.Deserialize<OneCallResponse>(response.Content);
                return MapToWeatherForecast(forecastResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWeatherForecastAsync: {ex.Message}");
                throw;
            }
        }

        private WeatherData MapToWeatherData(OpenWeatherResponse response)
        {
            return new WeatherData
            {
                Temperature = response.Main.Temp,
                FeelsLike = response.Main.FeelsLike,
                Humidity = response.Main.Humidity,
                Pressure = response.Main.Pressure,
                WindSpeed = response.Wind.Speed,
                WindDirection = DegreesToCardinal(response.Wind.Deg),
                Visibility = response.Visibility / 1000.0, // convert to km
                CloudCoverageTotal = response.Clouds.All,
                WeatherDescription = response.Weather[0].Description,
                WeatherIcon = response.Weather[0].Icon,
                Sunrise = DateTimeOffset.FromUnixTimeSeconds(response.Sys.Sunrise).LocalDateTime,
                Sunset = DateTimeOffset.FromUnixTimeSeconds(response.Sys.Sunset).LocalDateTime,
                Timestamp = DateTime.Now,
                Latitude = response.Coord.Lat,
                Longitude = response.Coord.Lon,
                LocationName = response.Name
            };
        }

        private WeatherForecast MapToWeatherForecast(OneCallResponse response)
        {
            var forecast = new WeatherForecast();

            // Map hourly forecast
            foreach (var hourly in response.Hourly)
            {
                forecast.HourlyForecast.Add(new WeatherData
                {
                    Temperature = hourly.Temp,
                    FeelsLike = hourly.FeelsLike,
                    Humidity = hourly.Humidity,
                    Pressure = hourly.Pressure,
                    WindSpeed = hourly.WindSpeed,
                    WindDirection = DegreesToCardinal(hourly.WindDeg),
                    CloudCoverageTotal = hourly.Clouds,
                    WeatherDescription = hourly.Weather[0].Description,
                    WeatherIcon = hourly.Weather[0].Icon,
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(hourly.Dt).LocalDateTime,
                    Latitude = response.Lat,
                    Longitude = response.Lon
                });
            }

            // Map daily forecast
            foreach (var daily in response.Daily)
            {
                forecast.DailyForecast.Add(new WeatherData
                {
                    Temperature = daily.Temp.Day,
                    FeelsLike = daily.FeelsLike.Day,
                    Humidity = daily.Humidity,
                    Pressure = daily.Pressure,
                    WindSpeed = daily.WindSpeed,
                    WindDirection = DegreesToCardinal(daily.WindDeg),
                    CloudCoverageTotal = daily.Clouds,
                    WeatherDescription = daily.Weather[0].Description,
                    WeatherIcon = daily.Weather[0].Icon,
                    Sunrise = DateTimeOffset.FromUnixTimeSeconds(daily.Sunrise).LocalDateTime,
                    Sunset = DateTimeOffset.FromUnixTimeSeconds(daily.Sunset).LocalDateTime,
                    MoonIllumination = daily.MoonPhase switch
                    {
                        0 or 1 => 0,
                        0.25 or 0.75 => 0.5,
                        0.5 => 1,
                        _ => daily.MoonPhase < 0.5 ? daily.MoonPhase * 2 : (1 - daily.MoonPhase) * 2
                    },
                    MoonPhase = GetMoonPhaseName(daily.MoonPhase),
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(daily.Dt).LocalDateTime,
                    Latitude = response.Lat,
                    Longitude = response.Lon
                });
            }

            return forecast;
        }

        private MoonPhase ExtractMoonPhase(OneCallResponse response, DateTime date)
        {
            var daily = response.Daily.FirstOrDefault(d => 
                DateTimeOffset.FromUnixTimeSeconds(d.Dt).Date == date.Date);

            if (daily == null)
                return null;

            var moonPhase = new MoonPhase
            {
                Date = DateTimeOffset.FromUnixTimeSeconds(daily.Dt).LocalDateTime,
                IlluminationPercentage = GetIlluminationPercentage(daily.MoonPhase),
                PhaseAngle = daily.MoonPhase * 360,
                PhaseName = GetMoonPhaseName(daily.MoonPhase),
                Moonrise = DateTimeOffset.FromUnixTimeSeconds(daily.Moonrise).LocalDateTime,
                Moonset = DateTimeOffset.FromUnixTimeSeconds(daily.Moonset).LocalDateTime
            };

            // Calculate days to next new and full moon
            CalculateDaysToMoonPhases(response, moonPhase);

            return moonPhase;
        }

        private double GetIlluminationPercentage(double moonPhase)
        {
            // Convert moon phase (0-1) to illumination percentage (0-100)
            return moonPhase switch
            {
                0 or 1 => 0,      // New moon
                0.5 => 100,       // Full moon
                _ => moonPhase < 0.5 
                    ? moonPhase * 2 * 100   // Waxing phases
                    : (1 - moonPhase) * 2 * 100  // Waning phases
            };
        }

        private void CalculateDaysToMoonPhases(OneCallResponse response, MoonPhase moonPhase)
        {
            var today = DateTimeOffset.Now.Date;
            
            // Find next new moon (phase = 0 or 1)
            var nextNewMoon = response.Daily
                .Where(d => Math.Abs(d.MoonPhase) < 0.05 || Math.Abs(d.MoonPhase - 1) < 0.05)
                .Select(d => DateTimeOffset.FromUnixTimeSeconds(d.Dt).Date)
                .Where(date => date >= today)
                .OrderBy(date => date)
                .FirstOrDefault();

            // Find next full moon (phase = 0.5)
            var nextFullMoon = response.Daily
                .Where(d => Math.Abs(d.MoonPhase - 0.5) < 0.05)
                .Select(d => DateTimeOffset.FromUnixTimeSeconds(d.Dt).Date)
                .Where(date => date >= today)
                .OrderBy(date => date)
                .FirstOrDefault();

            moonPhase.DaysToNextNewMoon = nextNewMoon != default 
                ? (nextNewMoon - today).Days 
                : -1;

            moonPhase.DaysToNextFullMoon = nextFullMoon != default 
                ? (nextFullMoon - today).Days 
                : -1;
        }

        private string GetMoonPhaseName(double moonPhase)
        {
            return moonPhase switch
            {
                var p when p is 0 or 1 or >= 0.97 => "New Moon",
                var p when p >= 0.01 && p < 0.24 => "Waxing Crescent",
                var p when p >= 0.24 && p < 0.26 => "First Quarter",
                var p when p >= 0.26 && p < 0.49 => "Waxing Gibbous",
                var p when p >= 0.49 && p < 0.51 => "Full Moon",
                var p when p >= 0.51 && p < 0.74 => "Waning Gibbous",
                var p when p >= 0.74 && p < 0.76 => "Last Quarter",
                var p when p >= 0.76 && p < 0.97 => "Waning Crescent",
                _ => "Unknown"
            };
        }

        private string DegreesToCardinal(double degrees)
        {
            string[] cardinals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
            return cardinals[(int)Math.Round(degrees % 360 / 45.0)];
        }

        #region JSON Response Classes
        
        private class OpenWeatherResponse
        {
            public Coord Coord { get; set; }
            public List<Weather> Weather { get; set; }
            public string Base { get; set; }
            public Main Main { get; set; }
            public int Visibility { get; set; }
            public Wind Wind { get; set; }
            public Clouds Clouds { get; set; }
            public long Dt { get; set; }
            public Sys Sys { get; set; }
            public int Timezone { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public int Cod { get; set; }
        }

        private class Coord
        {
            public double Lon { get; set; }
            public double Lat { get; set; }
        }

        private class Weather
        {
            public int Id { get; set; }
            public string Main { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
        }

        private class Main
        {
            public double Temp { get; set; }
            public double FeelsLike { get; set; }
            public double TempMin { get; set; }
            public double TempMax { get; set; }
            public int Pressure { get; set; }
            public int Humidity { get; set; }
        }

        private class Wind
        {
            public double Speed { get; set; }
            public int Deg { get; set; }
        }

        private class Clouds
        {
            public int All { get; set; }
        }

        private class Sys
        {
            public int Type { get; set; }
            public int Id { get; set; }
            public string Country { get; set; }
            public long Sunrise { get; set; }
            public long Sunset { get; set; }
        }

        private class OneCallResponse
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
            public string Timezone { get; set; }
            public int TimezoneOffset { get; set; }
            public List<HourlyForecast> Hourly { get; set; }
            public List<DailyForecast> Daily { get; set; }
        }

        private class HourlyForecast
        {
            public long Dt { get; set; }
            public double Temp { get; set; }
            public double FeelsLike { get; set; }
            public int Pressure { get; set; }
            public int Humidity { get; set; }
            public double DewPoint { get; set; }
            public double Uvi { get; set; }
            public int Clouds { get; set; }
            public int Visibility { get; set; }
            public double WindSpeed { get; set; }
            public int WindDeg { get; set; }
            public List<Weather> Weather { get; set; }
            public double Pop { get; set; }
        }

        private class DailyForecast
        {
            public long Dt { get; set; }
            public long Sunrise { get; set; }
            public long Sunset { get; set; }
            public long Moonrise { get; set; }
            public long Moonset { get; set; }
            public double MoonPhase { get; set; }
            public Temp Temp { get; set; }
            public FeelsLike FeelsLike { get; set; }
            public int Pressure { get; set; }
            public int Humidity { get; set; }
            public double DewPoint { get; set; }
            public double WindSpeed { get; set; }
            public int WindDeg { get; set; }
            public List<Weather> Weather { get; set; }
            public int Clouds { get; set; }
            public double Pop { get; set; }
            public double Uvi { get; set; }
        }

        private class Temp
        {
            public double Day { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public double Night { get; set; }
            public double Eve { get; set; }
            public double Morn { get; set; }
        }

        private class FeelsLike
        {
            public double Day { get; set; }
            public double Night { get; set; }
            public double Eve { get; set; }
            public double Morn { get; set; }
        }
        
        #endregion
    }
}
