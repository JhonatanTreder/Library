using API.Enum;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título do livro é obrigatório")]
        [StringLength(75, MinimumLength = 1, ErrorMessage = "O título deve conter entre 1 a 75 caracteres")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do autor é obrigatório")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O nome do autor deve conter entre 3 a 45 caracteres")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria do livro é obrigatória")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "A categoria deve conter entre 3 a 45 caracteres")]
        public string Category { get; set; } = string.Empty;

        [StringLength(400, MinimumLength = 1, ErrorMessage = "A descrição do livro deve conter entre 1 a 400 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O nome da editora é obrigatório")]
        [StringLength(40, ErrorMessage = "O nome da editora deve conter no máximo 40 caracteres")]
        public string Publisher { get; set; } = string.Empty;

        [Range(1440, 2999, ErrorMessage = "O ano de publicação deve estar entre 1440 a 2999")]
        public int PublicationYear { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
        public ICollection<FavoriteBook>? FavoritedBooks { get; set; }
    }
}
