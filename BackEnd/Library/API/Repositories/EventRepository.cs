using API.Context;
using API.DTO.Event;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<Event?> AddEventAsync(EventCreateDTO _event)
        {
            if (_event is null)
            {
                return null;
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

            return newEvent;
        }

        public async Task<EventResponse> DeleteEventAsync(int id)
        {
            var _event = await _context.Events.FindAsync(id);

            if (_event is null)
                return EventResponse.NotFound;

            _context.Events.Remove(_event);
            await _context.SaveChangesAsync();

            return EventResponse.Success;
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(EventFilterDTO eventFilterDTO)
        {
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

            return await query.ToListAsync();
        }

        public async Task<EventResponse> UpdateEventAsync(int id, EventUpdateDTO updateEventDTO)
        {
            if (updateEventDTO is null)
                return EventResponse.NullObject;

            var _event = await _context.Events.FindAsync(id);

            if (_event is null)
                return EventResponse.NotFound;

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

            return EventResponse.Success;
        }
    }
}
