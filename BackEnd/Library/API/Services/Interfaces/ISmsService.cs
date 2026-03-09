using API.DTOs.Authentication;
using API.Enum.Responses;

namespace API.Services.Interfaces
{
    public interface ISmsService
    {
        Task<RepositoryStatus> SendAsync(string phoneNumber, string message);
        Task<RepositoryStatus> VerifyAsync(VerifyPhoneCodeDTO verifyPhoneDTO);
    }
}
