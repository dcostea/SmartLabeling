using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SmartLabeling.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly ApiSettings _apiSettings;

        public MainController(ILogger<MainController> logger, ApiSettings apiSettings)
        {
            _logger = logger;
            _apiSettings = apiSettings;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"GET triggered with '{_apiSettings.Entry}'.");

            return Ok(_apiSettings.Entry);
        }
    }
}
