namespace AstroToolkit.Models
{
    public class MoonPhase
    {
        // The date of this moon phase
        public DateTime Date { get; set; }

        // Phase name (New Moon, Waxing Crescent, First Quarter, etc.)
        public string PhaseName { get; set; }

        // Illumination percentage (0-100)
        public double IlluminationPercentage { get; set; }

        // Angle of the moon phase (0-360 degrees)
        public double PhaseAngle { get; set; }

        // Days since the last new moon
        public int DaysSinceNewMoon { get; set; }

        // Days until next new moon
        public int DaysToNextNewMoon { get; set; }

        // Days until next full moon
        public int DaysToNextFullMoon { get; set; }

        // Moonrise time
        public DateTime? Moonrise { get; set; }

        // Moonset time
        public DateTime? Moonset { get; set; }

        // Moon's altitude above horizon (degrees)
        public double Altitude { get; set; }

        // Moon's azimuth (degrees)
        public double Azimuth { get; set; }

        // Moon's distance from Earth (km)
        public double Distance { get; set; }

        // Whether the moon is currently above the horizon
        public bool IsVisible => Altitude > 0;

        // Get a description of how this moon phase affects astrophotography
        public string GetAstroImpact()
        {
            if (IlluminationPercentage < 10)
                return "Excellent for deep sky astrophotography. Minimal light interference.";
            else if (IlluminationPercentage < 30)
                return "Good for deep sky photography. Some light interference.";
            else if (IlluminationPercentage < 60)
                return "Moderate light interference. Consider using filters.";
            else
                return "Significant moonlight. Best for lunar photography.";
        }

        // Get resource name for the appropriate moon phase icon
        public string GetPhaseIconResource()
        {
            if (IlluminationPercentage < 5)
                return "new_moon";
            else if (IlluminationPercentage < 45 && DaysSinceNewMoon < 15)
                return "waxing_crescent";
            else if (IlluminationPercentage >= 45 && IlluminationPercentage < 55 && DaysSinceNewMoon < 15)
                return "first_quarter";
            else if (IlluminationPercentage >= 55 && IlluminationPercentage < 95 && DaysSinceNewMoon < 15)
                return "waxing_gibbous";
            else if (IlluminationPercentage >= 95)
                return "full_moon";
            else if (IlluminationPercentage >= 55 && IlluminationPercentage < 95)
                return "waning_gibbous";
            else if (IlluminationPercentage >= 45 && IlluminationPercentage < 55)
                return "last_quarter";
            else
                return "waning_crescent";
        }
    }
}
