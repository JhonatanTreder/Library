using API.DTOs.Authentication;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace API.Services.Email
{
    public class BrevoEmailService : IEmailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public BrevoEmailService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<RepositoryStatus> SendAsync(string email, string subject, string message)
        {
            try
            {
                var senderName = _configuration["Brevo:SenderName"];
                var senderEmail = _configuration["Brevo:SenderEmail"];
                var apiKey = _configuration["Brevo:ApiKey"];

                Configuration.Default.ApiKey.Clear();
                Configuration.Default.ApiKey.Add("api-key", apiKey);

                var apiInstance = new TransactionalEmailsApi();

                var sendSmtpEmail = new SendSmtpEmail
                {
                    Sender = new SendSmtpEmailSender(senderName, senderEmail),
                    To = new List<SendSmtpEmailTo>
                    {
                        new SendSmtpEmailTo(email)
                    },

                    Subject = subject,
                    TextContent = message
                };

                var sendResult = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);

                if (sendResult.MessageId == null)
                    return RepositoryStatus.FailedToSendEmail;

                return RepositoryStatus.Success;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RepositoryStatus.Failed;
            }
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
    }
}
