using API.DTOs.EventDTOs;
using API.DTOs.Responses;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<RepositoryResponse<Event>> AddEventAsync(EventCreateDTO _eventCreateDTO);
        Task<RepositoryResponse<IEnumerable<Event>>> GetEventsAsync(EventFilterDTO eventFilterDTO);
        Task<RepositoryResponse<Event>> GetEventByIdAsync(int id);
        Task<RepositoryStatus> UpdateEventAsync(int id, EventUpdateDTO updateEventDTO);
        Task<RepositoryStatus> DeleteEventAsync(int id);
    }
}
