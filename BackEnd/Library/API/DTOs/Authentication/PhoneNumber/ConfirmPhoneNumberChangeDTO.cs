namespace API.DTOs.Authentication.PhoneNumber
{
    public class ConfirmPhoneNumberChangeDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string PhoneNumberCode { get; set; } = string.Empty;
    }
}
