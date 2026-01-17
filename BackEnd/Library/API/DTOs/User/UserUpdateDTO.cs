using System.ComponentModel.DataAnnotations;

namespace API.DTOs.User
{
    public class UserUpdateDTO
    {
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O nome deve estar entre 3 a 45 caracteres")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail deve estar em um formato válido")]
        public string? Email { get; set; }

        [StringLength(15, MinimumLength = 15, ErrorMessage = "A matrícula deve conter 15 caracteres")]
        public string? UserMatriculates { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
