using API.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("server-status")]
        [AllowAnonymous]
        public IActionResult CheckServerStatus()
        {
            return Ok(new ApiResponse
            {
                Status = "Ok",
                Data = null,
                Message = $"Servidor ativo - {DateTime.UtcNow.ToLocalTime()}"
            });
        }
    }
}
