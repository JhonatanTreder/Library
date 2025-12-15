using API.Enum;

namespace API.DTOs.BookDTOs
{
    public class BookReturnDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
