using System.ComponentModel.DataAnnotations;

namespace AstroToolkitWeb.Models
{
    public class SpotPhoto
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public string? UploadedByUserId { get; set; }

        // Physical path to the stored file (not exposed to clients)
        public string StoragePath { get; set; } = string.Empty;

        // URL path to access the photo
        public string? Url { get; set; }

        // Thumbnail URL
        public string? ThumbnailUrl { get; set; }

        // The AstroSpot this photo is associated with
        public int AstroSpotId { get; set; }
        
        // Navigation property
        public virtual AstroSpot? AstroSpot { get; set; }

        // Camera EXIF and technical details
        public string? CameraModel { get; set; }
        
        public string? Lens { get; set; }
        
        public double? FocalLength { get; set; }
        
        public double? Aperture { get; set; }
        
        public double? ExposureTime { get; set; }
        
        public int? ISO { get; set; }
        
        public DateTime? CaptureDate { get; set; }
        
        // Image dimensions
        public int? Width { get; set; }
        
        public int? Height { get; set; }
        
        // File size in bytes
        public long? FileSize { get; set; }
        
        // Rating (1-5 stars)
        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}