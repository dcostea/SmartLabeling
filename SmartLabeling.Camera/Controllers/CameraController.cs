using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartLabeling.Core.Models;

namespace SmartLabeling.Camera.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CameraController : ControllerBase
    {
        private readonly ILogger<CameraController> _logger;
        private readonly CameraSettings _cameraSettings;

        public CameraController(ILogger<CameraController> logger, CameraSettings cameraSettings)
        {
            _logger = logger;
            _cameraSettings = cameraSettings;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation($"GET triggered with '{_cameraSettings.Entry}'.");

            return Ok(_cameraSettings.Entry);
        }
    }
}
