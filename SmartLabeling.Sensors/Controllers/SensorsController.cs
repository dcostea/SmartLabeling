using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SmartLabeling.Sensors.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly ILogger<SensorsController> _logger;
        private readonly SensorsSettings _sensorsSettings;

        public SensorsController(ILogger<SensorsController> logger, SensorsSettings sensorsSettings)
        {
            _logger = logger;
            _sensorsSettings = sensorsSettings;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"GET triggered with '{_sensorsSettings.Entry}'.");

            return Ok(_sensorsSettings.Entry);
        }
    }
}
