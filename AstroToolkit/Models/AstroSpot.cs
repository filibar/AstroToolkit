using SQLite;

namespace AstroToolkit.Models
{
    [Table("AstroSpots")]
    public class AstroSpot
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        // Base64 encoded image data
        public string PhotoData { get; set; }

        // URL for navigation (e.g., Google Maps URL)
        public string NavigationUrl { get; set; }

        // Rating from 1 to 5
        public int Rating { get; set; }

        // Date the spot was added
        public DateTime CreatedDate { get; set; }

        // Optional example photos (stored as base64 strings)
        public string ExamplePhotos { get; set; }

        // Light pollution level (Bortle scale 1-9)
        public int LightPollutionLevel { get; set; }

        // Generate a Google Maps URL based on coordinates
        public string GetGoogleMapsUrl()
        {
            return $"https://www.google.com/maps/search/?api=1&query={Latitude},{Longitude}";
        }

        // Parse example photos from JSON string
        public List<string> GetExamplePhotosList()
        {
            if (string.IsNullOrEmpty(ExamplePhotos))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(ExamplePhotos);
            }
            catch
            {
                return new List<string>();
            }
        }

        // Set example photos list
        public void SetExamplePhotosList(List<string> photos)
        {
            ExamplePhotos = JsonSerializer.Serialize(photos);
        }
    }
}
