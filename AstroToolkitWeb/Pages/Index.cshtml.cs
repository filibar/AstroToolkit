using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AstroToolkitWeb.Models;
using AstroToolkitWeb.Services;

namespace AstroToolkitWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly LocationService _locationService;
        private readonly AstroCalculationService _astroService;

        public IndexModel(
            ILogger<IndexModel> logger,
            LocationService locationService,
            AstroCalculationService astroService)
        {
            _logger = logger;
            _locationService = locationService;
            _astroService = astroService;
        }

        public IEnumerable<AstroSpot> RecentSpots { get; private set; } = new List<AstroSpot>();
        
        public MoonPhase TonightMoonPhase { get; private set; } = new MoonPhase();

        public async Task OnGetAsync()
        {
            try
            {
                // Get the 3 most recent spots
                RecentSpots = (await _locationService.GetAllSpotsAsync())
                    .OrderByDescending(s => s.CreatedDate)
                    .Take(3)
                    .ToList();
                
                // Calculate tonight's moon phase
                TonightMoonPhase = _astroService.CalculateMoonPhase(DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading data for index page");
                RecentSpots = new List<AstroSpot>();
            }
        }
    }
}