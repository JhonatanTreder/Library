namespace API.DTOs.Authentication
{
    public class VerifyEmailCodeDTO
    {
        public string Email { get; set; } = string.Empty;
        public string EmailCode { get; set; } = string.Empty;
    }
}
