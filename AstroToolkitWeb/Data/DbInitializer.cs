using Microsoft.EntityFrameworkCore;
using AstroToolkitWeb.Models;
using AstroToolkitWeb.Services;

namespace AstroToolkitWeb.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            IServiceProvider serviceProvider,
            ILogger<ApplicationDbContext> logger)
        {
            try
            {
                // For in-memory database, we don't need migrations
                // Just ensure the database is created
                await context.Database.EnsureCreatedAsync();

                // Only seed if the database is empty
                if (!context.AstroSpots.Any())
                {
                    // Example spots across the globe
                    var spots = new List<AstroSpot>
                    {
                        new AstroSpot
                        {
                            Name = "Atacama Desert",
                            Description = "One of the darkest skies in the world with exceptional seeing conditions.",
                            Latitude = -23.4583,
                            Longitude = -70.6509,
                            LightPollutionLevel = 1, // Lowest light pollution (Bortle scale 1)
                            Rating = 5,
                            IsPublic = true,
                            Elevation = 2407,
                            CreatedDate = DateTime.UtcNow.AddDays(-30),
                            LastVisitDate = DateTime.UtcNow.AddDays(-5)
                        },
                        new AstroSpot
                        {
                            Name = "Mauna Kea",
                            Description = "High altitude Hawaiian peak with professional observatories and exceptional transparency.",
                            Latitude = 19.8207,
                            Longitude = -155.4680,
                            LightPollutionLevel = 2,
                            Rating = 5,
                            IsPublic = true,
                            Elevation = 4207,
                            CreatedDate = DateTime.UtcNow.AddDays(-60),
                            LastVisitDate = DateTime.UtcNow.AddDays(-15)
                        },
                        new AstroSpot
                        {
                            Name = "Cherry Springs State Park",
                            Description = "Dark sky park in Pennsylvania with good Eastern US viewing conditions.",
                            Latitude = 41.6561,
                            Longitude = -77.8269,
                            LightPollutionLevel = 3,
                            Rating = 4,
                            IsPublic = true,
                            Elevation = 710,
                            CreatedDate = DateTime.UtcNow.AddDays(-90),
                            LastVisitDate = DateTime.UtcNow.AddDays(-20)
                        },
                        new AstroSpot
                        {
                            Name = "Namib Desert",
                            Description = "Remote desert location with minimal humidity and excellent southern sky viewing.",
                            Latitude = -24.2967,
                            Longitude = 15.3532,
                            LightPollutionLevel = 1,
                            Rating = 5,
                            IsPublic = true,
                            Elevation = 1000,
                            CreatedDate = DateTime.UtcNow.AddDays(-120),
                            LastVisitDate = DateTime.UtcNow.AddDays(-25)
                        },
                        new AstroSpot
                        {
                            Name = "Lake Tekapo",
                            Description = "Part of the Aoraki Mackenzie International Dark Sky Reserve in New Zealand.",
                            Latitude = -43.8871,
                            Longitude = 170.5348,
                            LightPollutionLevel = 2,
                            Rating = 5,
                            IsPublic = true,
                            Elevation = 710,
                            CreatedDate = DateTime.UtcNow.AddDays(-150),
                            LastVisitDate = DateTime.UtcNow.AddDays(-30)
                        },
                        new AstroSpot
                        {
                            Name = "Death Valley",
                            Description = "US national park with designated dark sky status and low humidity.",
                            Latitude = 36.5323,
                            Longitude = -116.9325,
                            LightPollutionLevel = 2,
                            Rating = 4,
                            IsPublic = true,
                            Elevation = -86, // Below sea level
                            CreatedDate = DateTime.UtcNow.AddDays(-45),
                            LastVisitDate = DateTime.UtcNow.AddDays(-10)
                        }
                    };

                    foreach (var spot in spots)
                    {
                        context.AstroSpots.Add(spot);
                    }

                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} spots", spots.Count);
                }

                // If we don't have any moon phases, add some for the current month
                if (!context.MoonPhases.Any())
                {
                    var astroService = serviceProvider.GetRequiredService<AstroCalculationService>();
                    
                    // Get today's date
                    var today = DateTime.UtcNow.Date;
                    
                    // Calculate moon phases for the next 30 days
                    var moonPhases = new List<MoonPhase>();
                    for (int i = 0; i < 30; i++)
                    {
                        var date = today.AddDays(i);
                        var moonPhase = astroService.CalculateMoonPhase(date);
                        moonPhases.Add(moonPhase);
                    }
                    
                    context.MoonPhases.AddRange(moonPhases);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} moon phases", moonPhases.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}