using System.ComponentModel.DataAnnotations;

namespace API.DTOs.User
{
    public class UserUpdateDTO
    {
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O nome deve estar entre 3 a 45 caracteres")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail deve estar em um formato válido")]
        public string? Email { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "A senha deve conter de 3 a 20 caracteres")]
        public string? Password { get; set; }

        [RegularExpression(@"^\+55\d{10,11}$", ErrorMessage = "O número deve estar no formato E.164. Ex: +5521912345678")]
        public string? PhoneNumber { get; set; }
    }
}
