using Microsoft.EntityFrameworkCore;
using AstroToolkitWeb.Models;

namespace AstroToolkitWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AstroSpot> AstroSpots { get; set; } = null!;
        public DbSet<SpotPhoto> SpotPhotos { get; set; } = null!;
        public DbSet<WeatherData> WeatherData { get; set; } = null!;
        public DbSet<MoonPhase> MoonPhases { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure AstroSpot
            modelBuilder.Entity<AstroSpot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedByUserId).HasMaxLength(450);
                entity.Property(e => e.UserId).HasMaxLength(450);
                
                // Configure relationship with SpotPhoto
                entity.HasMany(e => e.Photos)
                      .WithOne(p => p.AstroSpot)
                      .HasForeignKey(p => p.AstroSpotId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SpotPhoto
            modelBuilder.Entity<SpotPhoto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.StoragePath).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.UploadedByUserId).HasMaxLength(450);
                entity.Property(e => e.Url).HasMaxLength(1000);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
                entity.Property(e => e.CameraModel).HasMaxLength(100);
                entity.Property(e => e.Lens).HasMaxLength(100);
            });

            // Configure WeatherData
            modelBuilder.Entity<WeatherData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LocationName).HasMaxLength(100);
                entity.Property(e => e.WeatherDescription).HasMaxLength(100);
                entity.Property(e => e.IconCode).HasMaxLength(50);
                
                // Create an index on latitude, longitude, and date for faster lookups
                entity.HasIndex(e => new { e.Latitude, e.Longitude, e.Date });
            });

            // Configure MoonPhase
            modelBuilder.Entity<MoonPhase>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PhaseName).HasMaxLength(50);
                
                // Create an index on date for faster lookups
                entity.HasIndex(e => e.Date);
            });
        }
    }
}