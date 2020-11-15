using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartLabeling.Sensors.Models;

namespace SmartLabeling.Sensors.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly ILogger<SensorsController> _logger;
        private readonly SensorsSettings _settings;

        public SensorsController(ILogger<SensorsController> logger, SensorsSettings sensorsSettings)
        {
            _logger = logger;
            _settings = sensorsSettings;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"GET triggered with '{_settings.Entry}'.");

            return Ok(_settings.Entry);
        }
    }
}
