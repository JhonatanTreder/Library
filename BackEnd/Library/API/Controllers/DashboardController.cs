using API.DTOs.Dashboard;
using API.DTOs.Responses;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dasboardService;
        public DashboardController(IDashboardService dashboardService) 
        {
           _dasboardService = dashboardService;
        }

        [HttpGet]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLibraryStats()
        {
            var response = await _dasboardService.GetLibraryStatsAsync();

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Status da biblioteca encontrado com sucesso."
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Ocorreu um erro ao buscar pelo status da biblioteca."
                })
            };
        }
    }
}
