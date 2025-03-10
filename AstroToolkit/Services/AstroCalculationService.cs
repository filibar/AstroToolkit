namespace AstroToolkit.Services
{
    public class AstroCalculationService
    {
        // Calculates the maximum exposure time using the "500 Rule"
        // to avoid star trails in astrophotography
        public double Calculate500Rule(double focalLength, double cropFactor = 1.0)
        {
            return 500.0 / (focalLength * cropFactor);
        }

        // Calculates the maximum exposure time using the "NPF Rule"
        // which is more accurate than the 500 Rule
        public double CalculateNpfRule(double focalLength, double aperture, double pixelPitch, double declination = 0)
        {
            // NPF Rule: t = (35 × f + 30 × p) ÷ (d × cos(δ))
            // where:
            // t is the exposure time in seconds
            // f is the focal ratio (focal length / aperture)
            // p is the pixel pitch in microns
            // d is the declination of the object being photographed in degrees
            
            double focalRatio = focalLength / aperture;
            double declinationRadians = declination * Math.PI / 180.0;
            double npf = (35.0 * focalRatio + 30.0 * pixelPitch) / (focalLength * Math.Cos(declinationRadians));
            
            return Math.Round(npf, 1);
        }

        // Calculates the field of view in degrees
        public double CalculateFieldOfView(double focalLength, double sensorWidth)
        {
            return 2 * Math.Atan(sensorWidth / (2 * focalLength)) * 180.0 / Math.PI;
        }

        // Calculate the position of Polaris in the polar scope
        public (double Angle, double Offset) CalculatePolarisPosition(DateTime time, double latitude, double longitude)
        {
            // This is a simplified calculation
            // For accurate calculations, we would need a proper astronomical library
            
            // Get the local sidereal time
            double lst = CalculateLocalSiderealTime(time, longitude);
            
            // Polaris coordinates (approximate)
            double polarisRA = 2.530; // Right ascension in hours
            double polarisDec = 89.264; // Declination in degrees
            
            // Calculate hour angle
            double ha = lst - polarisRA;
            
            // Convert to radians
            double haRad = ha * 15.0 * Math.PI / 180.0;
            double latRad = latitude * Math.PI / 180.0;
            double decRad = polarisDec * Math.PI / 180.0;
            
            // Calculate alt/az
            double sinAlt = Math.Sin(decRad) * Math.Sin(latRad) + Math.Cos(decRad) * Math.Cos(latRad) * Math.Cos(haRad);
            double alt = Math.Asin(sinAlt);
            
            double cosAz = (Math.Sin(decRad) - Math.Sin(latRad) * sinAlt) / (Math.Cos(latRad) * Math.Cos(alt));
            double az = Math.Acos(cosAz);
            
            if (Math.Sin(haRad) > 0)
            {
                az = 2 * Math.PI - az;
            }
            
            // Convert to the polar scope view
            double angle = (az * 180.0 / Math.PI + 90) % 360;
            
            // Offset is approximately the angular distance from the celestial pole
            double offset = 90 - polarisDec;
            
            return (angle, offset);
        }

        // Calculate the local sidereal time
        private double CalculateLocalSiderealTime(DateTime time, double longitude)
        {
            // J2000.0 epoch
            DateTime j2000 = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            
            // Days since J2000.0
            TimeSpan timeSinceJ2000 = time.ToUniversalTime() - j2000;
            double daysSinceJ2000 = timeSinceJ2000.TotalDays;
            
            // Greenwich mean sidereal time at 0h UTC
            double gmst = 18.697374558 + 24.06570982441908 * daysSinceJ2000;
            
            // Normalize to 0-24 hours
            gmst = gmst % 24;
            if (gmst < 0) gmst += 24;
            
            // Local sidereal time
            double lst = gmst + longitude / 15.0;
            
            // Normalize to 0-24 hours
            lst = lst % 24;
            if (lst < 0) lst += 24;
            
            return lst;
        }

        // Calculate the moon phase
        public double CalculateMoonPhase(DateTime date)
        {
            // This is a simple approximation
            // For accurate calculations, we would need a proper astronomical library
            
            // Average lunation (in days)
            const double lunarCycle = 29.53058867;
            
            // New moon reference (Jan 6, 2000)
            DateTime newMoonRef = new DateTime(2000, 1, 6, 18, 14, 0, DateTimeKind.Utc);
            
            // Calculate days since reference
            TimeSpan timeSinceRef = date.ToUniversalTime() - newMoonRef;
            double daysSinceRef = timeSinceRef.TotalDays;
            
            // Calculate phase
            double phase = (daysSinceRef % lunarCycle) / lunarCycle;
            
            return phase;
        }
        
        // Calculate the sensor crop factor based on dimensions
        public double CalculateCropFactor(double sensorWidth, double sensorHeight)
        {
            // Full frame reference: 36mm x 24mm
            double diagonalFF = Math.Sqrt(36 * 36 + 24 * 24);
            double diagonalSensor = Math.Sqrt(sensorWidth * sensorWidth + sensorHeight * sensorHeight);
            
            return diagonalFF / diagonalSensor;
        }
        
        // Convert degrees to DMS (Degrees, Minutes, Seconds) format
        public string DegreesToDMS(double degrees, bool isLatitude)
        {
            string direction = "";
            
            if (isLatitude)
            {
                direction = degrees >= 0 ? "N" : "S";
            }
            else
            {
                direction = degrees >= 0 ? "E" : "W";
            }
            
            degrees = Math.Abs(degrees);
            int d = (int)degrees;
            double minutes = (degrees - d) * 60;
            int m = (int)minutes;
            double seconds = (minutes - m) * 60;
            int s = (int)Math.Round(seconds);
            
            return $"{d}° {m}' {s}\" {direction}";
        }
    }
}
