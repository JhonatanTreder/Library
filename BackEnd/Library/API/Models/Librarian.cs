using API.Enum;
using System.ComponentModel.DataAnnotations;
namespace API.Models
{
    public class Librarian
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O nome deve estar entre 3 a 45 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O e-mail deve estar em um formato válido")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "O número de telefone deve estar em um formato válido")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de usuário é obrigatório")]
        public UserType Type { get; set; }

        public ICollection<Loan>? Loans { get; set; } 
        public ICollection<Event>? Events { get; set; } 
    }
}
