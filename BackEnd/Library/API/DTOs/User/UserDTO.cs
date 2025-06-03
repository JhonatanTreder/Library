using API.Enum;

namespace API.DTOs.User
{
    public class UserDTO
    {
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public UserType? UserType { get; set; }
    }
}
