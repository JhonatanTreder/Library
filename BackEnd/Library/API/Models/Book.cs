using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Publisher { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PublicationYear { get; set; }
    }
}
