namespace API.DTOs.Authentication.Password
{
    public class ConfirmPasswordChangeDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string PasswordCode { get; set; } = string.Empty;
    }
}
