namespace API.DTOs.Authentication.Email
{
    public class CancelEmailChangeDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
