using API.DTOs.Authentication;
using API.DTOs.Authentication.Email;
using API.DTOs.Authentication.Password;
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
        Task<RepositoryStatus> RequestEmailChangeAsync(RequestEmailChangeDTO emailChangeDTO);
        Task<RepositoryStatus> ConfirmEmailChangeAsync(ConfirmEmailChangeDTO confirmEmailChangeDTO);
        Task<RepositoryStatus> CancelEmailChangeAsync(CancelEmailChangeDTO cancelEmailChangeDTO);
        Task<RepositoryStatus> RequestPasswordChangeAsync(RequestPasswordChangeDTO passwordChangeDTO);
        Task<RepositoryStatus> ConfirmPasswordChangeAsync(ConfirmPasswordChangeDTO confirmPasswordChangeDTO);
        Task<RepositoryStatus> SendEmailConfirmationAsync(string email);
        Task<RepositoryStatus> SendPhoneConfirmationAsync(string email);
        Task<RepositoryStatus> VerifyEmailCodeAsync(VerifyEmailCodeDTO verifyEmailDTO);
        Task<RepositoryStatus> VerifyPhoneCodeAsync(VerifyPhoneCodeDTO verifyPhoneDTO);
        Task<RepositoryResponse<TokenDTO>> RefreshToken(TokenDTO tokenDTO);
        Task<RepositoryStatus> RevokeToken(string username);
    }
}
