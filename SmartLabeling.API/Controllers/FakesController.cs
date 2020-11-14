using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLabeling.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FakesController : ControllerBase
    {
        /// <summary>
        /// This controller provides fake data for sensors and camera (see Data folder)
        /// </summary>
        public FakesController()
        {

        }
    }
}
