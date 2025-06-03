using API.Context;
using API.DTOs.Event;
using API.DTOs.Responses;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class EventRepository : IEventRepository
    {
        public readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<Event>> AddEventAsync(EventCreateDTO _event)
        {
            if (_event is null)
            {
                return new RepositoryResponse<Event>(RepositoryStatus.NullObject);
            }

            var newEvent = new Event
            {
                Title = _event.Title,
                Description = _event.Description,
                TargetAudience = _event.TargetAudience,
                Location = _event.Location,
                StartDate = _event.StartDate,
                EndDate = _event.EndDate
            };

            await _context.AddAsync(newEvent);
            await _context.SaveChangesAsync();

            return new RepositoryResponse<Event>(RepositoryStatus.Success, newEvent);
        }

        public async Task<RepositoryStatus> DeleteEventAsync(int id)
        {
            var _event = await _context.Events.FindAsync(id);

            if (_event is null)
                return RepositoryStatus.NotFound;

            _context.Events.Remove(_event);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryResponse<Event>> GetEventByIdAsync(int id)
        {
            var _event = await _context.Events.FindAsync(id);

            if (_event is not null)
            {
                return new RepositoryResponse<Event>(RepositoryStatus.Success, _event);
            }

            else
            {
                return new RepositoryResponse<Event>(RepositoryStatus.NotFound);
            }
        }

        public async Task<RepositoryResponse<IEnumerable<Event>>> GetEventsAsync(EventFilterDTO eventFilterDTO)
        {
            if (eventFilterDTO is null)
            {
                return new RepositoryResponse<IEnumerable<Event>>(RepositoryStatus.NullObject);
            }

            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(eventFilterDTO.Title))
                query = query.Where(e => e.Title.Contains(eventFilterDTO.Title));

            if (!string.IsNullOrEmpty(eventFilterDTO.Location))
                query = query.Where(e => e.Location.Contains(eventFilterDTO.Location));

            if (!string.IsNullOrEmpty(eventFilterDTO.TargetAudience))
                query = query.Where(e => e.TargetAudience.Contains(eventFilterDTO.TargetAudience));

            if (eventFilterDTO.StartDate.HasValue)
                query = query.Where(e => e.StartDate >= eventFilterDTO.StartDate.Value);

            if (eventFilterDTO.EndDate.HasValue)
                query = query.Where(e => e.EndDate == eventFilterDTO.EndDate);

            var events = await query.ToListAsync();

            if (events.Count > 0)
            {
                return new RepositoryResponse<IEnumerable<Event>>(RepositoryStatus.Success, events);
            }

            else
            {
                return new RepositoryResponse<IEnumerable<Event>>(RepositoryStatus.NotFound);
            }
        }

        public async Task<RepositoryStatus> UpdateEventAsync(int id, EventUpdateDTO updateEventDTO)
        {
            if (updateEventDTO is null)
                return RepositoryStatus.NullObject;

            var _event = await _context.Events.FindAsync(id);

            if (_event is null)
                return RepositoryStatus.NotFound;

            if (!string.IsNullOrEmpty(updateEventDTO.Title))
                _event.Title = updateEventDTO.Title!;

            if (!string.IsNullOrEmpty(updateEventDTO.Description))
                _event.Description = updateEventDTO.Description;

            if (!string.IsNullOrEmpty(updateEventDTO.Location))
                _event.Location = updateEventDTO.Location!;

            if (!string.IsNullOrEmpty(updateEventDTO.TargetAudience))
                _event.TargetAudience = updateEventDTO.TargetAudience!;

            if (updateEventDTO.StartDate.HasValue)
                _event.StartDate = updateEventDTO.StartDate.Value;

            if (updateEventDTO.EndDate.HasValue)
                _event.EndDate = updateEventDTO.EndDate.Value;

            _context.Update(_event);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }
    }
}
