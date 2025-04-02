using System.ComponentModel.DataAnnotations;

namespace API.DTO.Login
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "O campo 'Email' é obrigatório.")]
        [EmailAddress(ErrorMessage = "O Email deve estar em um formato válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo 'Senha' é obrigatório.")]
        public string Password { get; set; } = string.Empty;
    }
}
