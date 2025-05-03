using API.Enum;

namespace API.DTOs.Book
{
    public class BookCopyReturnDTO
    {
        public int CopyId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PublicationYear { get; set; }

        public BookStatus Status { get; set; }
    }
}
