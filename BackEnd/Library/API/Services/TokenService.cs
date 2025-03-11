using API.Models;
using Microsoft.IdentityModel.JsonWebTokens;
namespace API.Services
{
    public class TokenService
    {
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JsonWebTokenHandler();
        }
    }
}
