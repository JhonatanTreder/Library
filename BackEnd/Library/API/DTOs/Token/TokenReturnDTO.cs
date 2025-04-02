namespace API.DTO.Token
{
    public class TokenReturnDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration;
    }
}
