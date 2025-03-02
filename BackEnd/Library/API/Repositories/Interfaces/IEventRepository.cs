using API.DTO.Event;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<Event> AddEvent(Event _event);
        Task<IEnumerable<Event>> GetEventsAsync(EventFilterDTO eventFilterDTO);
        Task<Event?> GetEventById(int id);
        Task<bool> UpdateEvent(int id, UpdateEventDTO updateEventDTO);
        Task<bool> DeleteEvent(int id);
    }
}
