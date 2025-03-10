using SQLite;

namespace AstroToolkit.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private string _databasePath => Path.Combine(FileSystem.AppDataDirectory, "astrodata.db");

        public DatabaseService()
        {
            Initialize();
        }

        private async void Initialize()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(_databasePath);
            await _database.CreateTableAsync<AstroSpot>();
        }

        // AstroSpot CRUD operations
        public async Task<List<AstroSpot>> GetAllAstroSpotsAsync()
        {
            await EnsureDatabaseInitialized();
            return await _database.Table<AstroSpot>().ToListAsync();
        }

        public async Task<AstroSpot> GetAstroSpotAsync(int id)
        {
            await EnsureDatabaseInitialized();
            return await _database.Table<AstroSpot>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<AstroSpot>> GetNearbyAstroSpotsAsync(double latitude, double longitude, double radiusKm = 50)
        {
            await EnsureDatabaseInitialized();
            var allSpots = await GetAllAstroSpotsAsync();
            
            return allSpots.Where(spot => 
                CalculateDistance(latitude, longitude, spot.Latitude, spot.Longitude) <= radiusKm
            ).ToList();
        }

        public async Task<int> SaveAstroSpotAsync(AstroSpot spot)
        {
            await EnsureDatabaseInitialized();
            if (spot.Id != 0)
            {
                return await _database.UpdateAsync(spot);
            }
            else
            {
                spot.CreatedDate = DateTime.Now;
                return await _database.InsertAsync(spot);
            }
        }

        public async Task<int> DeleteAstroSpotAsync(AstroSpot spot)
        {
            await EnsureDatabaseInitialized();
            return await _database.DeleteAsync(spot);
        }

        public async Task<int> UpdateSpotRatingAsync(int spotId, int rating)
        {
            await EnsureDatabaseInitialized();
            var spot = await GetAstroSpotAsync(spotId);
            if (spot != null)
            {
                spot.Rating = rating;
                return await _database.UpdateAsync(spot);
            }
            return 0;
        }

        private async Task EnsureDatabaseInitialized()
        {
            if (_database == null)
            {
                _database = new SQLiteAsyncConnection(_databasePath);
                await _database.CreateTableAsync<AstroSpot>();
            }
        }

        // Calculate distance between two coordinates using Haversine formula (in kilometers)
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371;
            
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EarthRadiusKm * c;
            
            return distance;
        }
        
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
