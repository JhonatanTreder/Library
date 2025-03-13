using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Services.Interfaces
{
    public interface ITokenService
    {
        JwtSecurityToken GenerateAcessToken(IEnumerable<Claim> claims, IConfiguration _config);
        string GenerateRefreshToken();
        ClaimsPrincipal GetClaimsFromExpiredToken(string token, IConfiguration _config);
    }
}
