using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SmartLabeling.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly AppSettings _appSettings;

        public MainController(ILogger<MainController> logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"GET triggered with '{_appSettings.Entry}'.");

            return Ok(_appSettings.Entry);
        }
    }
}
