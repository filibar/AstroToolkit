using System.ComponentModel.DataAnnotations;

namespace AstroToolkitWeb.Models
{
    public class MoonPhase
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        // Moon illumination percentage (0-100)
        [Range(0, 100)]
        public double Illumination { get; set; }

        // Moon phase angle in degrees (0-360)
        [Range(0, 360)]
        public double PhaseAngle { get; set; }

        // 0: New Moon, 1: Waxing Crescent, 2: First Quarter, 3: Waxing Gibbous
        // 4: Full Moon, 5: Waning Gibbous, 6: Last Quarter, 7: Waning Crescent
        [Range(0, 7)]
        public int PhaseType { get; set; }

        [StringLength(50)]
        public string? PhaseName { get; set; }

        public DateTime? MoonriseTime { get; set; }

        public DateTime? MoonsetTime { get; set; }

        // Moon's distance from Earth in kilometers
        public double? Distance { get; set; }

        // Diameter in arc minutes
        public double? AngularDiameter { get; set; }

        //Added properties to match the new code
        public double IlluminationPercentage { get; set; }
        public double DaysSinceNewMoon { get; set; }
        public int DaysToNextNewMoon { get; set; }
        public int DaysToNextFullMoon { get; set; }

        // Get resource name for the appropriate moon phase icon
        public string GetPhaseIconResource()
        {
            string iconName;

            if (IlluminationPercentage < 5)
                iconName = "new-moon";
            else if (IlluminationPercentage < 45 && DaysSinceNewMoon < 15)
                iconName = "waxing-crescent";
            else if (IlluminationPercentage >= 45 && IlluminationPercentage < 55 && DaysSinceNewMoon < 15)
                iconName = "first-quarter";
            else if (IlluminationPercentage >= 55 && IlluminationPercentage < 95 && DaysSinceNewMoon < 15)
                iconName = "waxing-gibbous";
            else if (IlluminationPercentage >= 95)
                iconName = "full-moon";
            else if (IlluminationPercentage >= 55 && IlluminationPercentage < 95)
                iconName = "waning-gibbous";
            else if (IlluminationPercentage >= 45 && IlluminationPercentage < 55)
                iconName = "last-quarter";
            else
                iconName = "waning-crescent";

            // List of available images (based on the assets we know exist)
            var availableImages = new[] { "new-moon", "full-moon", "waxing-crescent", "waxing-gibbous", "waning-crescent", "waning-gibbous", "first-quarter", "last-quarter" };

            // Return the icon name if it's available, otherwise return placeholder
            return availableImages.Contains(iconName) ? iconName : "placeholder";
        }
    }
}