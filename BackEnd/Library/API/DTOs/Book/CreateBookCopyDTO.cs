namespace API.DTOs.Book
{
    public class CreateBookCopyDTO
    {
        public int BookId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
