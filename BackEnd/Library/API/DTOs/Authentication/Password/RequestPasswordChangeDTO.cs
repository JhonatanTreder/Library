namespace API.DTOs.Authentication.Password
{
    public class RequestPasswordChangeDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
