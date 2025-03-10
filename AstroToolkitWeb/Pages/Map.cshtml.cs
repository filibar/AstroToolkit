using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AstroToolkitWeb.Models;
using AstroToolkitWeb.Services;

namespace AstroToolkitWeb.Pages
{
    public class MapModel : PageModel
    {
        private readonly ILogger<MapModel> _logger;
        private readonly LocationService _locationService;
        private readonly IConfiguration _configuration;

        public MapModel(
            ILogger<MapModel> logger,
            LocationService locationService,
            IConfiguration configuration)
        {
            _logger = logger;
            _locationService = locationService;
            _configuration = configuration;
        }

        public List<AstroSpot> Spots { get; private set; } = new List<AstroSpot>();
        
        public string MapsApiKey => _configuration["GoogleMaps:ApiKey"] ?? "";
        
        public int? SelectedSpotId { get; private set; }

        public async Task OnGetAsync(int? spotId)
        {
            try
            {
                // Get all available spots
                Spots = (await _locationService.GetAllSpotsAsync()).ToList();
                
                // If a specific spot ID is requested, set it as selected
                if (spotId.HasValue)
                {
                    SelectedSpotId = spotId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading map data");
                Spots = new List<AstroSpot>();
            }
        }
    }
}