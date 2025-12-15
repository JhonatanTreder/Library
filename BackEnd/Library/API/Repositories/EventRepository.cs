using API.Context;
using API.DTOs.EventDTOs;
using API.DTOs.Responses;
using API.Enum;
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

            var now = DateTime.UtcNow;
            var today = now.Date;

            EventStatus status;

            if (_event.StartDate >= _event.EndDate)
            {
                return new RepositoryResponse<Event>(RepositoryStatus.InvalidStartDate);
            }

            if (_event.EndDate < now)
            {
                return new RepositoryResponse<Event>(RepositoryStatus.InvalidEndDate);
            }

            if (_event.StartDate <= now && _event.EndDate > now)
            {
                status = EventStatus.InProgress;
            }

            else if (_event.StartDate > now)
            {
                status = EventStatus.Upcoming;
            }

            else
            {
                status = EventStatus.InProgress;
            }

            var newEvent = new Event
            {
                Title = _event.Title,
                Description = _event.Description,
                TargetAudience = _event.TargetAudience,
                Location = _event.Location,
                StartDate = _event.StartDate,
                EndDate = _event.EndDate,
                Status = status,
                IsArchived = false
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


        //-----------------------------------------------------------------------------------------------------------------
        public async Task<RepositoryResponse<IEnumerable<EventReturnDTO>>> GetActiveEventsAsync(int? activeDays = 7)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var now = DateTime.UtcNow;

                DateTime maxDate;

                if (activeDays.HasValue)
                {
                    maxDate = today.AddDays(activeDays.Value);
                }

                else
                {
                    maxDate = DateTime.MaxValue;
                }

                var inactiveStatuses = new List<EventStatus>()
                {
                    EventStatus.Cancelled,
                    EventStatus.Finished,
                    EventStatus.Archived,
                    EventStatus.Inactive,
                };

                
                var activeEvents = await _context.Events
                    .Where(e => !e.IsArchived
                        && !inactiveStatuses.Contains(e.Status)
                        && e.StartDate <= maxDate
                        && e.EndDate >= now)
                    .Select(e => new EventReturnDTO
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description ?? string.Empty,
                        TargetAudience = e.TargetAudience,
                        Location = e.Location,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Status = e.Status,
                    })
                    .OrderBy(e => e.StartDate)
                    .ToListAsync();

                if (activeEvents.Any())
                {
                    return new RepositoryResponse<IEnumerable<EventReturnDTO>>(RepositoryStatus.Success, activeEvents);
                }

                return new RepositoryResponse<IEnumerable<EventReturnDTO>>(RepositoryStatus.NotFound);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Erro em GetActiveEventsAsync: {ex.Message}");
                return new RepositoryResponse<IEnumerable<EventReturnDTO>>(RepositoryStatus.DatabaseError);
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

            bool datesChanged = false;

            if (updateEventDTO.StartDate.HasValue)
            {
                _event.StartDate = updateEventDTO.StartDate.Value;
                datesChanged = true;
            }

            if (updateEventDTO.EndDate.HasValue)
            {
                _event.EndDate = updateEventDTO.EndDate.Value;
            }

            if (datesChanged)
            {
                var today = DateTime.UtcNow.Date;

                if (_event.EndDate.Date < today)
                {
                    _event.Status = EventStatus.Finished;
                }

                else if (_event.StartDate.Date <= today && _event.EndDate.Date >= today)
                {
                    _event.Status = EventStatus.InProgress;
                }

                else if (_event.StartDate.Date >= today)
                {
                    _event.Status = EventStatus.Upcoming;
                }
            }

            _context.Update(_event);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> CancelEventAsync(int eventId, string cancellationReason)
        {
            var _event = await _context.Events.FindAsync(eventId);

            if (_event is null)
                return RepositoryStatus.NotFound;

            _event.Status = EventStatus.Cancelled;
            _event.CancelledAt = DateTime.UtcNow;
            _event.CancellationReason = cancellationReason;

            try
            {
                _context.Update(_event);
                await _context.SaveChangesAsync();
            }

            catch (Exception)
            {
                return RepositoryStatus.Failed;
            }

            return RepositoryStatus.Success;
        }

        public async Task<int> UpdateEventsStatusAsync()
        {
            var today = DateTime.UtcNow.Date;

            var events = await _context.Events
                .Where(e => !e.IsArchived &&
                           e.Status != EventStatus.Cancelled &&
                           e.Status != EventStatus.Archived)
                .ToListAsync();

            foreach (var e in events)
            {
                var newStatus = e.Status;

                if (e.EndDate.Date < today)
                {
                    newStatus = EventStatus.Finished;
                }

                else if (e.StartDate.Date <= today && e.EndDate.Date >= today)
                {
                    newStatus = EventStatus.InProgress;
                }

                else if (e.StartDate.Date > today)
                {
                    newStatus = EventStatus.Upcoming;
                }

                if (e.Status != newStatus)
                {
                    e.Status = newStatus;
                }
            }

            var changes = await _context.SaveChangesAsync();
            return changes;
        }

        public async Task<int> ArchiveOldEventsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var archiveThreshold = today.AddDays(-30);

            var eventsToArchive = await _context.Events
                .Where(e => !e.IsArchived &&
                           (e.Status == EventStatus.Finished || e.Status == EventStatus.Cancelled) &&
                           e.EndDate.Date < archiveThreshold)
                .ToListAsync();

            foreach (var e in eventsToArchive)
            {
                e.IsArchived = true;
                e.Status = EventStatus.Archived;
            }

            if (eventsToArchive.Any())
            {
                await _context.SaveChangesAsync();
            }

            return eventsToArchive.Count;
        }
    }
}
