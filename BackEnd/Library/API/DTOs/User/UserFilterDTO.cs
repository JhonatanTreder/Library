using API.Enum;

namespace API.DTO.User
{
    public class UserFilterDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public UserType? UserType { get; set; }
    }
}
