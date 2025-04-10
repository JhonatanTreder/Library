using API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public ClaimsPrincipal GetClaimsFromExpiredToken(string token)
        {
            var secretKey = GetPrivateKey();
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                                                        out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                                !jwtSecurityToken.Header.Alg.Equals(
                                SecurityAlgorithms.HmacSha256,
                                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = Encoding.UTF8.GetBytes(GetPrivateKey());

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _config["JWT:ValidIssuer"],
                Audience = _config["JWT:ValidAudience"],
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JWT:TokenValidityInMinutes"] ?? "30")),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (JwtSecurityToken)token;
        }

        public string GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[128];

            using var randomNumberGenerator = RandomNumberGenerator.Create();

            randomNumberGenerator.GetBytes(secureRandomBytes);

            return Base64UrlEncoder.Encode(secureRandomBytes);
        }

        private string GetPrivateKey()
        {
            return _config["JWT:SecretKey"] ?? throw new InvalidOperationException("Invalid key");
        }
    }
}
