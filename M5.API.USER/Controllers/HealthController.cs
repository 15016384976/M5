﻿using Microsoft.AspNetCore.Mvc;

namespace M5.API.LINK.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet(nameof(Detect))]
        public IActionResult Detect()
        {
            return Ok();
        }
    }
}
