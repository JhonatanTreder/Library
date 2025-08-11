using API.DTOs.Authentication;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace API.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SmtpEmailService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<RepositoryStatus> SendAsync(string email)
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

        public async Task<RepositoryStatus> VerifyAsync(VerifyEmailCodeDTO verifyEmailDTO)
        {
            var user = await _userManager.FindByEmailAsync(verifyEmailDTO.Email);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            if (user.EmailConfirmationCode != verifyEmailDTO.EmailCode)
                return RepositoryStatus.InvalidConfirmationCode;

            user.EmailConfirmed = true;
            user.EmailConfirmationCode = null;

            var updateUser = await _userManager.UpdateAsync(user);

            if (updateUser.Succeeded is false)
            {
                user.EmailConfirmationCodeExpiryTime = DateTime.UtcNow;

                var resetUser = await _userManager.UpdateAsync(user);

                if (resetUser.Succeeded is false)
                    return RepositoryStatus.FailedToUpdateUser;

                return RepositoryStatus.FailedToUpdateUser;
            }

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
    }
}
