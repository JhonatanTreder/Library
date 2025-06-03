using API.Enum;

namespace API.DTOs.User
{
    public class UserFilterDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public UserType? UserType { get; set; }
    }
}
