using API.Enum;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace API.DTOs.Book
{
    public class BookReturnDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public BookStatus Status { get; set; }
    }
}
