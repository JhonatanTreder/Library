using API.DTOs.Authentication;
using API.DTOs.Responses;
using API.DTOs.Token;
using API.DTOs.User;
using API.Enum;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using DnsClient;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsServcie;

        public AuthService(ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IEmailService emailService,
            ISmsService smsService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _smsServcie = smsService;
        }

        public async Task<RepositoryResponse<TokenReturnDTO>> Login(LoginDTO loginDTO)
        {
            if (loginDTO is null)
            {
                return new RepositoryResponse<TokenReturnDTO>(RepositoryStatus.NullObject);
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email!);

            if (user is not null && await _userManager.CheckPasswordAsync(user, loginDTO.Password!))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.UserType.ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _tokenService.GenerateAccessToken(authClaims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    return new RepositoryResponse<TokenReturnDTO>(RepositoryStatus.FailedToUpdateUser);
                }

                var tokenInfo = new TokenReturnDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                };

                return new RepositoryResponse<TokenReturnDTO>(RepositoryStatus.Success, tokenInfo);
            }

            return new RepositoryResponse<TokenReturnDTO>(RepositoryStatus.Unauthorized);
        }

        public async Task<RepositoryResponse<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (registerDTO is null)
            {
                return new RepositoryResponse<UserDTO>(RepositoryStatus.NullObject);
            }

            string[] emailParts = registerDTO.Email.Split('@');
            var domain = emailParts[1];

            var validDomain = await IsValidDomain(domain);

            if (validDomain is false)
            {
                return new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidDomain);
            }

            var userExists = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (userExists != null)
            {
                return new RepositoryResponse<UserDTO>(RepositoryStatus.EmailAlreadyExists);
            }

            if (registerDTO.Password.Length < 6)
            {
                return new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidPassword);
            }

            ApplicationUser user = new()
            {
                Email = registerDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerDTO.Name,
                CreatedAt = DateOnly.FromDateTime(DateTime.Today)
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                return new RepositoryResponse<UserDTO>(RepositoryStatus.FailedToCreateUser);
            }

            var roleName = UserType.User.ToString().ToLower();

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (!roleResult.Succeeded)
                {
                    return new RepositoryResponse<UserDTO>(RepositoryStatus.FailedToCreateRole);
                }
            }

            var roleAssignmentResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!roleAssignmentResult.Succeeded)
            {
                return new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidRoleAssignment);
            }

            var userInfo = new UserDTO
            {
                Name = registerDTO.Name,
                Email = registerDTO.Email,
                UserType = UserType.User.ToString()
            };

            return new RepositoryResponse<UserDTO>(RepositoryStatus.Success, userInfo);
        }

        public async Task<RepositoryResponse<TokenDTO>> RefreshToken(TokenDTO tokenDTO)
        {
            if (tokenDTO is null)
            {
                return new RepositoryResponse<TokenDTO>(RepositoryStatus.NullObject);
            }

            string? accessToken = tokenDTO.AccessToken ?? throw new ArgumentNullException(nameof(tokenDTO));
            string? refreshToken = tokenDTO.RefreshToken ?? throw new ArgumentNullException(nameof(tokenDTO));

            var principal = _tokenService.GetClaimsFromExpiredToken(accessToken!);

            if (principal is null)
            {
                return new RepositoryResponse<TokenDTO>(RepositoryStatus.InvalidClaims);
            }

            string username = principal.Identity!.Name!;

            var user = await _userManager.FindByNameAsync(username!);

            if (user is null)
                return new RepositoryResponse<TokenDTO>(RepositoryStatus.UserNotFound);

            if (string.IsNullOrEmpty(user.RefreshToken)
                || user.RefreshToken != refreshToken)
                return new RepositoryResponse<TokenDTO>(RepositoryStatus.InvalidRefreshToken);

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return new RepositoryResponse<TokenDTO>(RepositoryStatus.InvalidRefreshTokenExpiryTime);

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList());
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return new RepositoryResponse<TokenDTO>(RepositoryStatus.FailedToUpdateUser);
            }

            var tokenInfo = new TokenDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            };

            return new RepositoryResponse<TokenDTO>(RepositoryStatus.Success, tokenInfo);
        }

        public async Task<RepositoryStatus> RevokeToken(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return RepositoryStatus.NullObject;
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                return RepositoryStatus.UserNotFound;
            }

            user.RefreshToken = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return RepositoryStatus.FailedToRevokeToken;
            }

            return RepositoryStatus.Success; ;
        }

        public async Task<RepositoryStatus> SendEmailConfirmationAsync(string email)
        {
            return await _emailService.SendAsync(email);
        }

        public async Task<RepositoryStatus> SendPhoneConfirmationAsync(string email)
        {
            return await _smsServcie.SendAsync(email);
        }

        public async Task<RepositoryStatus> VerifyEmailCodeAsync(VerifyEmailCodeDTO verifyEmailDTO)
        {
            return await _emailService.VerifyAsync(verifyEmailDTO);
        }

        public async Task<RepositoryStatus> VerifyPhoneCodeAsync(VerifyPhoneCodeDTO verifyPhoneDTO)
        {
            return await _smsServcie.VerifyAsync(verifyPhoneDTO);
        }

        private async Task<bool> IsValidDomain(string domain)
        {
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domain, QueryType.MX);

            return result.Answers.MxRecords().Any();
        }
    }
}
