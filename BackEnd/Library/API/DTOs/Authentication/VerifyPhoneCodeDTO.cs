namespace API.DTOs.Authentication
{
    public class VerifyPhoneCodeDTO
    {
        public string Email { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
    }
}
