namespace API.Models
{
    public class FavoriteBook
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

}
