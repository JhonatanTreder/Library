using API.DTOs.Authentication;
using API.DTOs.Authentication.Email;
using API.DTOs.Authentication.Password;
using API.DTOs.Responses;
using API.DTOs.Token;
using API.DTOs.User;
using API.Enum;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using API.Utils.Validators;
using DnsClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Twilio.Rest.Numbers.V2.RegulatoryCompliance;
using Twilio.TwiML.Voice;

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
                return new RepositoryResponse<UserDTO>(RepositoryStatus.NullObject);

            string[] emailParts = registerDTO.Email.Split('@');

            var domain = emailParts[1];
            var validDomain = await FormatValidator.ValidateDomainFormat(domain);

            if (validDomain is false)
                return new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidDomain);

            var userExists = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (userExists != null)
                return new RepositoryResponse<UserDTO>(RepositoryStatus.EmailAlreadyExists);

            if (registerDTO.Password.Length < 6)
                return new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidPassword);

            if (FormatValidator.ValidateMatriculatesFormat(registerDTO.Matriculates) is false)
                return new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidMatriculatesFormat);

            var userWithMatriculates = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Matriculates == registerDTO.Matriculates);

            if (userWithMatriculates != null)
                return new RepositoryResponse<UserDTO>(RepositoryStatus.MatriculatesAlreadyExists);

            ApplicationUser user = new()
            {
                UserName = registerDTO.Email,
                Name = registerDTO.Name,
                Email = registerDTO.Email,
                Matriculates = registerDTO.Matriculates,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateOnly.FromDateTime(DateTime.Today)
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                if (result.Errors.Any())
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine(registerDTO.Name);
                        Console.WriteLine(user.Name);
                        Console.WriteLine(error.Code);
                        Console.WriteLine(error.Description);
                        Console.WriteLine("--------------------------------------------------");
                    }

                    var errorDTO = new UserDTO
                    {
                        UserErrors = result
                    };

                    return new RepositoryResponse<UserDTO>(RepositoryStatus.FailedToCreateUser, errorDTO);
                }

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

        public async Task<RepositoryStatus> RequestEmailChangeAsync(RequestEmailChangeDTO emailChangeDTO)
        {
            var user = await _userManager.FindByIdAsync(emailChangeDTO.UserId);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            if (user.Email is null)
                return RepositoryStatus.EmailNotFound;

            var passwordCheck = await _userManager.CheckPasswordAsync(user, emailChangeDTO.UserPassword);

            if (passwordCheck is false)
                return RepositoryStatus.InvalidPassword;

            var emailExists = await _userManager.FindByEmailAsync(emailChangeDTO.NewEmail);

            if (emailExists != null)
                return RepositoryStatus.EmailAlreadyExists;

            var tokenGenerator = new Random();
            var confirmationCode = tokenGenerator.Next(100000, 999999);

            user.EmailConfirmationCode = confirmationCode.ToString();
            user.EmailConfirmationCodeExpiryTime = DateTime.UtcNow.AddMinutes(5);
            user.PendingEmail = emailChangeDTO.NewEmail;

            var cancelToken = Guid.NewGuid().ToString("N");

            user.EmailChangeCancelToken = cancelToken;
            user.EmailChangeCancelTokenExpiryTime = DateTime.UtcNow.AddMinutes(10);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return RepositoryStatus.FailedToUpdateUser;

            var subject = "Confirmação de Alteração de Email";
            var message = $"Seu código de confirmação para alteração de email é: {confirmationCode}. " +
                "Este código é válido por 5 minutos.";

            var sendResult = await _emailService.SendAsync(emailChangeDTO.NewEmail, subject, message);

            if (sendResult != RepositoryStatus.Success)
                return RepositoryStatus.Failed;

            var cancelLink = $"{_configuration["FrontEnd:BaseURL"]}/cancel-email-change" +
                             $"?userId={user.Id}&token={cancelToken}";

            var emailChangeSubject = "Alerta de Segurança: ";
            var emailChangeMessage = $@"
            Recebemos uma solicitação para alterar o e-mail da sua conta.
            Novo e-mail solicitado: {emailChangeDTO.NewEmail}
            Se você NÃO realizou essa solicitação, cancele imediatamente clicando no link abaixo:

            {cancelLink}

            Se foi você, pode ignorar este e-mail.";

            await _emailService.SendAsync(user.Email, emailChangeSubject, emailChangeMessage);

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> CancelEmailChangeAsync(CancelEmailChangeDTO cancelEmailChangeDTO)
        {
            var user = await _userManager.FindByIdAsync(cancelEmailChangeDTO.UserId);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            if (string.IsNullOrEmpty(user.PendingEmail))
                return RepositoryStatus.PendingEmailNotFound;

            if (user.EmailChangeCancelToken != cancelEmailChangeDTO.Token)
                return RepositoryStatus.InvalidToken;

            if (user.EmailChangeCancelTokenExpiryTime < DateTime.UtcNow)
                return RepositoryStatus.ExpiredToken;

            user.PendingEmail = null;
            user.EmailChangeCancelToken = null;
            user.EmailChangeCancelTokenExpiryTime = DateTime.UtcNow;

            user.EmailConfirmationCode = null;
            user.EmailConfirmationCodeExpiryTime = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return RepositoryStatus.FailedToUpdateUser;

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> ConfirmEmailChangeAsync(ConfirmEmailChangeDTO emailChangeDTO)
        {
            var user = await _userManager.FindByIdAsync(emailChangeDTO.UserId);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            if (user.EmailConfirmationCode != emailChangeDTO.EmailCode)
                return RepositoryStatus.InvalidConfirmationCode;

            if (user.EmailConfirmationCodeExpiryTime < DateTime.UtcNow)
                return RepositoryStatus.ExpiredConfirmationCode;

            if (user.PendingEmail is null)
                return RepositoryStatus.EmailNotFound;

            var newEmail = user.PendingEmail;

            user.Email = newEmail;
            user.UserName = newEmail;
            user.NormalizedEmail = newEmail.ToUpper();
            user.NormalizedUserName = newEmail.ToUpper();
            user.EmailConfirmed = true;
            user.EmailConfirmationCode = null;
            user.EmailConfirmationCodeExpiryTime = DateTime.UtcNow;
            user.PendingEmail = null;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return RepositoryStatus.FailedToUpdateUser;

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> RequestPasswordChangeAsync(RequestPasswordChangeDTO passwordChangeDTO)
        {
            var user = await _userManager.FindByIdAsync(passwordChangeDTO.UserId);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            var passwordCheck = await _userManager.CheckPasswordAsync(user, passwordChangeDTO.CurrentPassword);

            if (!passwordCheck)
                return RepositoryStatus.InvalidPassword;

            if (FormatValidator.ValidatePasswordFormat(passwordChangeDTO.NewPassword) is false)
                return RepositoryStatus.InvalidPassword;

            var tokenGenerator = new Random();
            var confirmationCode = tokenGenerator.Next(100000, 999999);

            user.PasswordChangeCode = confirmationCode.ToString();
            user.PasswordChangeCodeExpiryTime = DateTime.UtcNow.AddMinutes(5);
            user.PendingPassword = passwordChangeDTO.NewPassword;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return RepositoryStatus.FailedToUpdateUser;

            if (user.Email == null)
                return RepositoryStatus.EmailNotFound;

            var subject = "Confirmação de Alteração de Senha";
            var message = $"Seu código de confirmação para alteração de senha é: {confirmationCode}. " +
                "Este código é válido por 5 minutos.";

            var sendResult = await _emailService.SendAsync(user.Email, subject, message);

            if (sendResult != RepositoryStatus.Success)
                return RepositoryStatus.Failed;

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> ConfirmPasswordChangeAsync(ConfirmPasswordChangeDTO passwordChangeDTO)
        {
            var user = await _userManager.FindByIdAsync(passwordChangeDTO.UserId);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            if (string.IsNullOrEmpty(user.PasswordChangeCode))
                return RepositoryStatus.InvalidConfirmationCode;

            if (user.PasswordChangeCode != passwordChangeDTO.PasswordCode)
                return RepositoryStatus.InvalidConfirmationCode;

            if (user.PasswordChangeCodeExpiryTime < DateTime.UtcNow)
            {
                user.PasswordChangeCode = null;
                user.PasswordChangeCodeExpiryTime = DateTime.UtcNow;
                user.PendingPassword = null;

                await _userManager.UpdateAsync(user);

                return RepositoryStatus.ExpiredConfirmationCode;
            }

            var newPassword = user.PendingPassword;

            if (string.IsNullOrEmpty(newPassword))
                return RepositoryStatus.Failed;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!resetResult.Succeeded)
                return RepositoryStatus.FailedToResetPassword;

            user.PasswordChangeCode = null;
            user.PasswordChangeCodeExpiryTime = DateTime.UtcNow;
            user.PendingPassword = null;

            // Invalidar refresh atual - Para permitir novo login por segurança
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return RepositoryStatus.FailedToUpdateUser;

            return RepositoryStatus.Success;
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

            var subject = "Confirmação de Email";
            var message = $"Seu código de confirmação de email é: {confirmationCode}. " +
                "Este código é válido por 5 minutos.";

            return await _emailService.SendAsync(email, subject, message);
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
    }
}
