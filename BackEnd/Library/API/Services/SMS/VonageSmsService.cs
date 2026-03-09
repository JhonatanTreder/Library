using API.DTOs.Authentication;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using API.Utils.Validators;
using Microsoft.AspNetCore.Identity;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace API.Services.SMS
{
    public class VonageSmsService : ISmsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public VonageSmsService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<RepositoryStatus> SendAsync(string phoneNumber, string message)
        {
            try
            {
                var vonageApiKey = _configuration["Vonage:VonageApiKey"];
                var vonageSecretKey = _configuration["Vonage:VonageSecretKey"];
                var credentials = Credentials.FromApiKeyAndSecret(vonageApiKey, vonageSecretKey);

                Console.WriteLine(credentials.ApiKey);
                Console.WriteLine(credentials.ApiSecret);
                Console.WriteLine(credentials.ToString());
                var smsRequestInfo = new SendSmsRequest()
                {
                    To = phoneNumber,
                    From = _configuration["Vonage:SenderName"],
                    Text = message
                };
                var vonageClient = new VonageClient(credentials);
                var vonageResponse = await vonageClient.SmsClient.SendAnSmsAsync(smsRequestInfo);

                if (vonageResponse.Messages.Any( s => s.StatusCode != SmsStatusCode.Success))
                    return RepositoryStatus.FailedToSendSMS;

                return RepositoryStatus.Success;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RepositoryStatus.Failed;
            }
        }

        public async Task<RepositoryStatus> VerifyAsync(VerifyPhoneCodeDTO verifyPhoneDTO)
        {
            var user = await _userManager.FindByEmailAsync(verifyPhoneDTO.Email);

            if (user is null) return RepositoryStatus.UserNotFound;
            if (user.PhoneNumber is null) return RepositoryStatus.NotFound;

            if (FormatValidator.ValidateE164Format(user.PhoneNumber) is false)
                return RepositoryStatus.InvalidPhoneFormat;

            if (user.PhoneConfirmationCodeExpiryTime < DateTime.UtcNow)
                return RepositoryStatus.ExpiredConfirmationCode;

            if (user.PhoneConfirmationCode != verifyPhoneDTO.PhoneCode)
                return RepositoryStatus.InvalidConfirmationCode;

            user.PhoneNumberConfirmed = true;
            user.PhoneConfirmationCode = null;
            user.PhoneConfirmationCodeExpiryTime = DateTime.UtcNow;

            var updateUser = await _userManager.UpdateAsync(user);

            if (!updateUser.Succeeded) return RepositoryStatus.FailedToUpdateUser;

            return RepositoryStatus.Success;
        }
    }
}
