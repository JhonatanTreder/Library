using API.Models;

namespace API.DTOs.User
{
    public class UserDashboardDTO
    {
        public int TotalLoans { get; set; }
        public ICollection<Event>? EventsHeld { get; set; }
        public ICollection<FavoriteBook>? FavoriteBooks { get; set; }
        public ICollection<BookCopy>? ReturnedBooks { get; set; }
        public decimal TotalFines { get; set; }
        public DateOnly EntryDate { get; set; }
    }
}
