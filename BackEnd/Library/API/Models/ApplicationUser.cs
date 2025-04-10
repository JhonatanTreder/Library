using API.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O nome deve estar entre 3 a 45 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de usuário é obrigatório")]
        public UserType UserType { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public ICollection<Loan>? Loans { get; set; }
        public ICollection<Event>? Events { get; set; }
    }
}
