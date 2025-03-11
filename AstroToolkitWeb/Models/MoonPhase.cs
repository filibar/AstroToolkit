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
        
        // Get resource name for the appropriate moon phase icon
        public string GetPhaseIconResource()
        {
            if (Illumination < 5)
                return "new-moon";
            else if (Illumination < 45 && DaysSinceNewMoon < 15)
                return "waxing-crescent";
            else if (Illumination >= 45 && Illumination < 55 && DaysSinceNewMoon < 15)
                return "first-quarter";
            else if (Illumination >= 55 && Illumination < 95 && DaysSinceNewMoon < 15)
                return "waxing-gibbous";
            else if (Illumination >= 95)
                return "full-moon";
            else if (Illumination >= 55 && Illumination < 95)
                return "waning-gibbous";
            else if (Illumination >= 45 && Illumination < 55)
                return "last-quarter";
            else
                return "waning-crescent";
        }
        
        // Get the icon path for the current phase
        public string GetPhaseIconPath()
        {
            return $"images/moon/{GetPhaseIconResource()}.svg";
        }

        //Added properties to match the changed function
        public double IlluminationPercentage { get; set; }
        public double DaysSinceNewMoon { get; set; }
    }
}