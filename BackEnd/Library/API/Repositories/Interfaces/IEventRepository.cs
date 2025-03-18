using API.DTO.Event;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> AddEventAsync(EventCreateDTO _eventCreateDTO);
        Task<IEnumerable<Event>> GetEventsAsync(EventFilterDTO eventFilterDTO);
        Task<Event?> GetEventByIdAsync(int id);
        Task<bool> UpdateEventAsync(int id, EventUpdateDTO updateEventDTO);
        Task<bool> DeleteEventAsync(int id);
    }
}
