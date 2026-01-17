using API.DTOs.Authentication;
using API.Enum.Responses;

namespace API.Services.Interfaces
{
    public interface IEmailService
    {
        Task<RepositoryStatus> SendAsync(string email, string subject, string message);
        Task<RepositoryStatus> VerifyAsync(VerifyEmailCodeDTO verifyPhoneDTO);
    }
}
