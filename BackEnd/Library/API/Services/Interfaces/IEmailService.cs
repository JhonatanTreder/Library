using API.DTOs.Authentication;
using API.Enum.Responses;

namespace API.Services.Interfaces
{
    public interface IEmailService
    {
        Task<RepositoryStatus> SendAsync(string email);
        Task<RepositoryStatus> VerifyAsync(VerifyEmailCodeDTO verifyPhoneDTO);
    }
}
