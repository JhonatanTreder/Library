using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Services.Interfaces
{
    public interface ITokenService
    {
        JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetClaimsFromExpiredToken(string token);
    }
}
