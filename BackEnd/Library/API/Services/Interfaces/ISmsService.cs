using API.DTOs.Authentication;
using API.Enum.Responses;

namespace API.Services.Interfaces
{
    public interface ISmsService
    {
        Task<RepositoryStatus> SendAsync(string email);
        Task<RepositoryStatus> VerifyAsync(VerifyPhoneCodeDTO verifyPhoneDTO);
    }
}
