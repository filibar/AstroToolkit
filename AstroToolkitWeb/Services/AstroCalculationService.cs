using AstroToolkitWeb.Models;

namespace AstroToolkitWeb.Services
{
    public class AstroCalculationService
    {
        private readonly ILogger<AstroCalculationService> _logger;

        public AstroCalculationService(ILogger<AstroCalculationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Calculates the "500 Rule" for astrophotography to determine maximum exposure time.
        /// </summary>
        /// <param name="focalLength">The focal length of the lens in millimeters</param>
        /// <param name="cropFactor">The crop factor of the camera sensor (1.0 for full frame)</param>
        /// <returns>Maximum exposure time in seconds before star trails appear</returns>
        public double Calculate500Rule(double focalLength, double cropFactor = 1.0)
        {
            if (focalLength <= 0)
            {
                throw new ArgumentException("Focal length must be greater than zero", nameof(focalLength));
            }

            if (cropFactor <= 0)
            {
                throw new ArgumentException("Crop factor must be greater than zero", nameof(cropFactor));
            }

            // The 500 rule: 500 / (focal length * crop factor)
            double exposureTime = 500 / (focalLength * cropFactor);
            
            // Round to one decimal place
            return Math.Round(exposureTime, 1);
        }

        /// <summary>
        /// Calculates the "NPF Rule" which is a more accurate version of the 500 rule.
        /// </summary>
        /// <param name="focalLength">The focal length of the lens in millimeters</param>
        /// <param name="aperture">The aperture of the lens (f-number)</param>
        /// <param name="pixelPitch">The pixel pitch of the camera sensor in microns</param>
        /// <param name="declination">The declination of the star in degrees (0 for celestial equator, 90 for pole)</param>
        /// <returns>Maximum exposure time in seconds before star trails appear</returns>
        public double CalculateNPFRule(double focalLength, double aperture, double pixelPitch, double declination = 0)
        {
            if (focalLength <= 0)
            {
                throw new ArgumentException("Focal length must be greater than zero", nameof(focalLength));
            }

            if (aperture <= 0)
            {
                throw new ArgumentException("Aperture must be greater than zero", nameof(aperture));
            }

            if (pixelPitch <= 0)
            {
                throw new ArgumentException("Pixel pitch must be greater than zero", nameof(pixelPitch));
            }

            if (declination < -90 || declination > 90)
            {
                throw new ArgumentException("Declination must be between -90 and 90 degrees", nameof(declination));
            }

            // Convert declination to radians
            double declinationRad = declination * Math.PI / 180;
            
            // NPF rule formula: (35 * aperture + 30 * pixelPitch) / (focalLength * Math.Cos(declinationRad))
            double exposureTime = (35 * aperture + 30 * pixelPitch) / (focalLength * Math.Cos(declinationRad));
            
            // Round to one decimal place
            return Math.Round(exposureTime, 1);
        }

        /// <summary>
        /// Calculates the field of view in degrees for a given focal length and sensor size.
        /// </summary>
        /// <param name="focalLength">The focal length of the lens in millimeters</param>
        /// <param name="sensorWidth">The width of the sensor in millimeters</param>
        /// <param name="sensorHeight">The height of the sensor in millimeters</param>
        /// <returns>A tuple containing (horizontal FOV, vertical FOV) in degrees</returns>
        public (double horizontal, double vertical) CalculateFieldOfView(double focalLength, double sensorWidth, double sensorHeight)
        {
            if (focalLength <= 0)
            {
                throw new ArgumentException("Focal length must be greater than zero", nameof(focalLength));
            }

            if (sensorWidth <= 0)
            {
                throw new ArgumentException("Sensor width must be greater than zero", nameof(sensorWidth));
            }

            if (sensorHeight <= 0)
            {
                throw new ArgumentException("Sensor height must be greater than zero", nameof(sensorHeight));
            }

            // Calculate horizontal FOV: 2 * arctan(sensorWidth / (2 * focalLength))
            double horizontalFov = 2 * Math.Atan(sensorWidth / (2 * focalLength)) * 180 / Math.PI;
            
            // Calculate vertical FOV: 2 * arctan(sensorHeight / (2 * focalLength))
            double verticalFov = 2 * Math.Atan(sensorHeight / (2 * focalLength)) * 180 / Math.PI;
            
            // Round to one decimal place
            return (Math.Round(horizontalFov, 1), Math.Round(verticalFov, 1));
        }

        /// <summary>
        /// Calculates the moon phase for a given date.
        /// </summary>
        /// <param name="date">The date to calculate the moon phase for</param>
        /// <returns>A MoonPhase object with the moon phase information</returns>
        public MoonPhase CalculateMoonPhase(DateTime date)
        {
            try
            {
                // Base this calculation on a known new moon date: January 6, 2000
                DateTime referenceNewMoon = new DateTime(2000, 1, 6, 18, 14, 0, DateTimeKind.Utc);
                
                // Lunar cycle is approximately 29.53059 days
                const double lunarCycle = 29.53059;
                
                // Calculate how many days have passed since the reference new moon
                TimeSpan daysSinceReference = date.ToUniversalTime() - referenceNewMoon;
                
                // Calculate the phase as a fraction of the lunar cycle (0 to 1)
                double phaseDay = (daysSinceReference.TotalDays % lunarCycle) / lunarCycle;
                
                // Adjust phase to be between 0 and 1
                if (phaseDay < 0) phaseDay += 1.0;
                
                // Convert to angle (0 to 360 degrees)
                double phaseAngle = phaseDay * 360.0;
                
                // Calculate illumination (0 to 100%)
                // This is an approximation: illumination = 50 * (1 - cos(phaseAngle in radians))
                double illumination = 50.0 * (1.0 - Math.Cos(phaseAngle * Math.PI / 180.0));
                
                // Determine phase type (0-7) and name
                int phaseType;
                string phaseName;
                
                if (phaseDay < 0.025 || phaseDay >= 0.975) // New Moon (within ~0.7 days)
                {
                    phaseType = 0;
                    phaseName = "New Moon";
                }
                else if (phaseDay < 0.225) // Waxing Crescent
                {
                    phaseType = 1;
                    phaseName = "Waxing Crescent";
                }
                else if (phaseDay < 0.275) // First Quarter (within ~1.5 days)
                {
                    phaseType = 2;
                    phaseName = "First Quarter";
                }
                else if (phaseDay < 0.475) // Waxing Gibbous
                {
                    phaseType = 3;
                    phaseName = "Waxing Gibbous";
                }
                else if (phaseDay < 0.525) // Full Moon (within ~1.5 days)
                {
                    phaseType = 4;
                    phaseName = "Full Moon";
                }
                else if (phaseDay < 0.725) // Waning Gibbous
                {
                    phaseType = 5;
                    phaseName = "Waning Gibbous";
                }
                else if (phaseDay < 0.775) // Last Quarter (within ~1.5 days)
                {
                    phaseType = 6;
                    phaseName = "Last Quarter";
                }
                else // Waning Crescent
                {
                    phaseType = 7;
                    phaseName = "Waning Crescent";
                }
                
                // Create and return the MoonPhase object
                return new MoonPhase
                {
                    Date = date.Date,
                    PhaseAngle = Math.Round(phaseAngle, 2),
                    Illumination = Math.Round(illumination, 2),
                    PhaseType = phaseType,
                    PhaseName = phaseName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating moon phase for date {Date}", date);
                
                // Return a default moon phase with error indicator
                return new MoonPhase
                {
                    Date = date.Date,
                    PhaseAngle = 0,
                    Illumination = 0,
                    PhaseType = 0,
                    PhaseName = "Error calculating moon phase"
                };
            }
        }
    }
}