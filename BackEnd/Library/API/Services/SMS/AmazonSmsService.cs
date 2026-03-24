using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using API.DTOs.Authentication;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using API.Utils.Validators;
using Microsoft.AspNetCore.Identity;
using sib_api_v3_sdk.Api;

namespace API.Services.SMS
{
    public class AmazonSmsService : ISmsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AmazonSmsService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<RepositoryStatus> SendAsync(string phoneNumber, string message)
        {
            var region = _configuration["AmazonSMS:Region"];
            var awsAccessKey = _configuration["AmazonSMS:AccessKey"];
            var awsSecretKey = _configuration["AmazonSMS:SecretKey"];

            var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);

            var config = new AmazonSimpleNotificationServiceConfig
            {
                RegionEndpoint = RegionEndpoint.SAEast1
            };

            var amazonSnsService = new AmazonSimpleNotificationServiceClient(credentials, config);

            var request = new PublishRequest
            {
                PhoneNumber = phoneNumber,
                Message = message
            };

            var response = await amazonSnsService.PublishAsync(request);


            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine(response.HttpStatusCode);
            Console.WriteLine(response.ContentLength);

            Console.WriteLine("Message:");
            Console.WriteLine(request.PhoneNumber);
            Console.WriteLine(request.Message);

            return RepositoryStatus.Success;
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
