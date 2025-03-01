namespace API.DTO.Book
{
    public class FilterBookDTO
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public string? Publisher { get; set; }
        public int? PublicationYear { get; set; }
    }
}
