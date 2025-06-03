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
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
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
                Console.WriteLine(validDomain);
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
                UserName = registerDTO.Name
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
                UserType = UserType.User
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
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            var tokenGenerator = new Random();

            var confirmationCode = tokenGenerator.Next(100000, 999999);

            user.EmailConfirmationCode = confirmationCode.ToString();
            user.EmailConfirmationCodeExpiryTime = DateTime.UtcNow.AddMinutes(5);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return RepositoryStatus.FailedToUpdateUser;

            string message = $"Seu código de confirmação: {confirmationCode}";
            string subject = "Confirmação de Email";

            await SendEmail(email, subject, message);

            return RepositoryStatus.Success;
        }

        private async Task<RepositoryStatus> SendEmail(string email, string subject, string body)
        {
            string mail = _configuration["SMTP:UserName"] ?? string.Empty;
            string name = _configuration["SMTP:Name"] ?? string.Empty;
            string password = _configuration["SMTP:Password"] ?? string.Empty;
            string host = _configuration["SMTP:Host"] ?? string.Empty;
            int port = Convert.ToInt32(_configuration["SMTP:Port"] ?? "587");

            MailMessage emailService = new()
            {
                From = new MailAddress(mail, name),
            };

            emailService.To.Add(email);
            emailService.Subject = subject;
            emailService.Body = body;
            emailService.IsBodyHtml = true;
            emailService.HeadersEncoding = Encoding.UTF8;
            emailService.BodyEncoding = Encoding.UTF8;
            emailService.Priority = MailPriority.High;

            using (SmtpClient smtpClient = new SmtpClient(host, port))
            {
                smtpClient.Credentials = new NetworkCredential(mail, password);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(emailService);

                return RepositoryStatus.Success;
            }
        }

        public async Task<RepositoryStatus> VerifyEmailCodeAsync(VerifyEmailCodeDTO verifyEmailDTO)
        {
            var user = await _userManager.FindByEmailAsync(verifyEmailDTO.Email);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            if (user.EmailConfirmationCode != verifyEmailDTO.EmailCode)
                return RepositoryStatus.InvalidConfirmationCode;

            user.EmailConfirmed = true;

            var updateUser = await _userManager.UpdateAsync(user);

            if (updateUser.Succeeded is false)
            {
                user.EmailConfirmationCode = null;
                user.EmailConfirmationCodeExpiryTime = DateTime.UtcNow;

                var resetUser = await _userManager.UpdateAsync(user);

                if (resetUser.Succeeded is false)
                    return RepositoryStatus.FailedToUpdateUser;

                return RepositoryStatus.FailedToUpdateUser;
            }

            return RepositoryStatus.Success;
        }

        private async Task<bool> IsValidDomain(string domain)
        {
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domain, QueryType.MX);

            Console.WriteLine(result.Answers.MxRecords().Any());

            return result.Answers.MxRecords().Any();
        }
    }
}
