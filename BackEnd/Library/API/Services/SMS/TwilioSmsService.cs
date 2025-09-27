using API.DTOs.Authentication;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using API.Utils.Validators;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace API.Services.SMS
{
    public class TwilioSmsService : ISmsService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TwilioSmsService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RepositoryStatus> SendAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return RepositoryStatus.UserNotFound;
            if (user.PhoneNumber is null) return RepositoryStatus.NotFound;

            if (PhoneNumberValidator.ValidateE164Format(user.PhoneNumber) is false)
                return RepositoryStatus.InvalidPhoneFormat;

            var code = RandomNumberGenerator.GetInt32(100_000, 999_999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            user.PhoneConfirmationCode = code;
            user.PhoneConfirmationCodeExpiryTime = expiry;
            user.PhoneNumberConfirmed = false;

            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded) return RepositoryStatus.FailedToUpdateUser;

            var sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            var from = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER");

            TwilioClient.Init(sid, token);

            var result = await MessageResource.CreateAsync(
                to: user.PhoneNumber,
                from: from,
                body: $"O seu código de verificação é: {code}");

            Console.WriteLine(result);
            Console.WriteLine(result.Status);
            Console.WriteLine(result.ErrorCode);
            Console.WriteLine(result.ErrorMessage);
            Console.WriteLine(result.Body);

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> VerifyAsync(VerifyPhoneCodeDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null) return RepositoryStatus.UserNotFound;
            if (user.PhoneNumber is null) return RepositoryStatus.NotFound;

            if (PhoneNumberValidator.ValidateE164Format(user.PhoneNumber) is false)
                return RepositoryStatus.InvalidPhoneFormat;

            if (user.PhoneConfirmationCodeExpiryTime < DateTime.UtcNow)
                return RepositoryStatus.ConfirmationCodeExpired;

            if (user.PhoneConfirmationCode != dto.PhoneCode)
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
