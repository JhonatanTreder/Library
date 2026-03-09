namespace API.DTOs.Authentication.PhoneNumber
{
    public class RequestPhoneNumberChangeDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string NewPhoneNumber { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
    }
}
