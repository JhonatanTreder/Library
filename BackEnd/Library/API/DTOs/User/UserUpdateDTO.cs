using System.ComponentModel.DataAnnotations;

namespace API.DTO.User
{
    public class UserUpdateDTO
    {
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O nome deve estar entre 3 a 45 caracteres")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail deve estar em um formato válido")]
        public string? Email { get; set; }
        
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "A senha deve conter de 3 a 20 caracteres")]
        public string? Password { get; set; }

        [Phone(ErrorMessage = "O número de telefone deve estar em um formator válido")]
        public string? PhoneNumber { get; set; }
    }
}
