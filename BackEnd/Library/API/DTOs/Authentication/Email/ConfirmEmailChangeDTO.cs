namespace API.DTOs.Authentication.Email
{
    public class ConfirmEmailChangeDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string EmailCode { get; set; } = string.Empty;
    }
}
