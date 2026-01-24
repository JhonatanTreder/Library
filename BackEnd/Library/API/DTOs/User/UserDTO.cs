using API.Enum;
using Microsoft.AspNetCore.Identity;

namespace API.DTOs.User
{
    public class UserDTO
    {
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? UserType { get; set; }
        public string? Matriculates { get; set; }

        public IdentityResult? UserErrors { get; set; }
    }
}
