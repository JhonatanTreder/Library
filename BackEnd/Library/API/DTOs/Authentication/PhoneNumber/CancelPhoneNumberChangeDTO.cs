namespace API.DTOs.Authentication.PhoneNumber
{
    public class CancelPhoneNumberChangeDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
