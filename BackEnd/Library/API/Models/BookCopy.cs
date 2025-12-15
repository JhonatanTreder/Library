using API.Enum;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class BookCopy
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O status da cópia é obrigatório.")]
        [EnumDataType(typeof(BookStatus), ErrorMessage = "O status deve estar em um formato válido.")]
        public BookStatus Status { get; set; }

        [Required(ErrorMessage = "O livro associado à cópia é obrigatório.")]
        public int BookId { get; set; }

        public Book? Book { get; set; }

        public ICollection<Loan>? Loans { get; set; }

        public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    }
}
