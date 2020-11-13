using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SmartLabeling.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("GET triggered.");

            return Ok();
        }
    }
}
