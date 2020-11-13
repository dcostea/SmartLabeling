using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLabeling.API.Controllers
{
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { status = "OK" });
        }

        //[HttpGet("health")]
        //public IActionResult Health()
        //{
        //    return Ok(new { status = "OK" });
        //}
    }
}
