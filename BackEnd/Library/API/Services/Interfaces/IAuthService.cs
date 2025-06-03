using API.DTOs.Authentication;
using API.DTOs.Responses;
using API.DTOs.Token;
using API.DTOs.User;
using API.Enum.Responses;

namespace API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RepositoryResponse<TokenReturnDTO>> Login(LoginDTO loginDTO);
        Task<RepositoryResponse<UserDTO>> Register(RegisterDTO registerDTO);
        Task<RepositoryStatus> SendEmailConfirmationAsync(string email);
        Task<RepositoryStatus> VerifyEmailCodeAsync(VerifyEmailCodeDTO verifyEmailDTO);
        Task<RepositoryResponse<TokenDTO>> RefreshToken(TokenDTO tokenDTO);
        Task<RepositoryStatus> RevokeToken(string username);
    }
}
