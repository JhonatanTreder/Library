using API.DTOs.BookDTOs;
using API.DTOs.EventDTOs;

namespace API.DTOs.Dashboard
{
    public class LibraryStatsDTO
    {
        public IEnumerable<BookReturnDTO>? RecentBooks { get; set; }
        public IEnumerable<BookReturnDTO>? TotalBooks { get; set; }
        public IEnumerable<BookReturnDTO>? DelayedBooks { get; set; }
        public IEnumerable<BookCopyReturnDTO>? UnavailableBooks { get; set; }
        public IEnumerable<EventReturnDTO>? ActiveEvents { get; set; }
    }
}
