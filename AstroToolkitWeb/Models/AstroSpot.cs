using System.ComponentModel.DataAnnotations;

namespace AstroToolkitWeb.Models
{
    public class AstroSpot
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Range(1, 9)]
        public int LightPollutionLevel { get; set; } = 5;

        [Range(0, 5)]
        public int Rating { get; set; } = 0;

        public double? Elevation { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastVisitDate { get; set; }

        public string? CreatedByUserId { get; set; }
        
        public string? UserId { get; set; }

        public bool IsPublic { get; set; } = true;

        public virtual ICollection<SpotPhoto> Photos { get; set; } = new List<SpotPhoto>();
    }
}