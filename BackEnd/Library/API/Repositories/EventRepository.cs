using API.Context;
using API.DTO.Event;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API.Repositories
{
    public class EventRepository : IEventRepository
    {
        public readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event> AddEvent(Event _event)
        {
            await _context.AddAsync(_event);
            await _context.SaveChangesAsync();

            return _event;
        }

        public async Task<bool> DeleteEvent(int id)
        {
            var _event = await _context.Events.FindAsync(id);

            if(_event is null)
            {
                return false;
            }

            _context.Events.Remove(_event);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Event?> GetEventById(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(EventFilterDTO eventFilterDTO)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(eventFilterDTO.Title))
                query = query.Where(e => e.Title == eventFilterDTO.Title);

            if (!string.IsNullOrEmpty(eventFilterDTO.Location))
                query = query.Where(e => e.Location == eventFilterDTO.Location);

            if (!string.IsNullOrEmpty(eventFilterDTO.TargetAudience))
                query = query.Where(e => e.TargetAudience == eventFilterDTO.TargetAudience);

            if (eventFilterDTO.StartDate.HasValue)
                query = query.Where(e => e.StartDate >= eventFilterDTO.StartDate.Value);


            if (eventFilterDTO.EndDate.HasValue)
                query = query.Where(e => e.EndDate == eventFilterDTO.EndDate);

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateEvent(int id, UpdateEventDTO updateEventDTO)
        {
            var _event = await _context.Events.FindAsync(id);

            if(_event is null)
            {
                return false;
            }

            _event.Title = updateEventDTO.Title!;
            _event.Description = updateEventDTO.Description;
            _event.Location = updateEventDTO.Location!;
            _event.TargetAudience = updateEventDTO.TargetAudience!;

            _context.Update(_event);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
