namespace API.DTOs.Authentication.Email
{
    public class RequestEmailChangeDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
    }
}
