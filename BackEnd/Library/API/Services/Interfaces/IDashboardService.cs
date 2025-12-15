using API.DTOs.Dashboard;
using API.DTOs.Responses;

namespace API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<RepositoryResponse<LibraryStatsDTO>> GetLibraryStatsAsync();
    }
}
