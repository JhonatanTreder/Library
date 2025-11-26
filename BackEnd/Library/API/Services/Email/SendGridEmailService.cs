using API.DTOs.Authentication;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace API.Services.Email
{
    public class SendGridEmailService : IEmailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SendGridEmailService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
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

            string subject = "Confirmação de Email";
            string message = $"Seu código de confirmação: {confirmationCode}";

            var sendResult = await SendEmail(email, subject, message);

            return sendResult;
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

        private async Task<RepositoryStatus> SendEmail(string recipient, string subject, string body)
        {
            var apiKey = _configuration["SendGrid:ApiKey"] ?? string.Empty;
            var senderEmail = _configuration["SendGrid:SenderEmail"] ?? string.Empty;
            var senderName = _configuration["SendGrid:SenderName"] ?? string.Empty;

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(recipient);

            var message = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            try
            {
                var sendEmailResponse = await client.SendEmailAsync(message);

                if (!sendEmailResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erro do SendGrid: {sendEmailResponse.StatusCode}");
                    Console.WriteLine($"Response: {sendEmailResponse}");
                    Console.WriteLine($"Body do SendGrid: {sendEmailResponse.Body}");
                    Console.WriteLine($"Headers: {sendEmailResponse.Headers}");
                    return RepositoryStatus.Failed;
                }

                return RepositoryStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar email viaSendGrid: {ex.Message}");
                return RepositoryStatus.Failed;
            }
        }
    }
}
