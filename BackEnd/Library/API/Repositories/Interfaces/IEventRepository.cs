using API.DTO.Event;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> AddEventAsync(EventCreateDTO _eventCreateDTO);
        Task<IEnumerable<Event>> GetEventsAsync(EventFilterDTO eventFilterDTO);
        Task<Event?> GetEventByIdAsync(int id);
        Task<EventResponse> UpdateEventAsync(int id, EventUpdateDTO updateEventDTO);
        Task<EventResponse> DeleteEventAsync(int id);
    }
}
