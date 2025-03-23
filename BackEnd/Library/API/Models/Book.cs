using API.Enum;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título do livro é obrigatório")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O título deve conter entre 1 a 50 caracteres")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do autor é obrigatório")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O nome do autor deve conter entre 3 a 45 caracteres")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria do livro é obrigatória")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "A categoria deve conter entre 3 a 20 caracteres")]
        public string Category { get; set; } = string.Empty;

        [StringLength(400, MinimumLength = 1, ErrorMessage = "A descrição do livro deve conter entre 1 a 400 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O nome da editora é obrigatória")]
        [StringLength(40, ErrorMessage = "O nome da editora deve conter no máximo 40 caracteres")]
        public string Publisher { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1")]
        public int? Quantity { get; set; }

        [Range(1440, 2999, ErrorMessage = "O ano de publicação deve estar entre 1440 a 2999")]
        public int PublicationYear { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório.")]
        [EnumDataType(typeof(LoanStatus), ErrorMessage = "O status deve estar em um formato válido.")]
        public LoanStatus Status { get; set; }

        public ICollection<Loan>? Loans { get; set; }
    }
}
