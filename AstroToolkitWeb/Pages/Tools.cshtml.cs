using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AstroToolkitWeb.Services;

namespace AstroToolkitWeb.Pages
{
    public class ToolsModel : PageModel
    {
        private readonly ILogger<ToolsModel> _logger;
        private readonly AstroCalculationService _astroService;

        public ToolsModel(
            ILogger<ToolsModel> logger,
            AstroCalculationService astroService)
        {
            _logger = logger;
            _astroService = astroService;
        }

        public void OnGet()
        {
            // This page is primarily client-side, so there's minimal server-side processing
            _logger.LogInformation("Tools page accessed");
        }
    }
}