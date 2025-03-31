using API.DTO.Login;
using API.DTO.Responses;
using API.DTO.Token;
using API.DTO.User;
using API.Enum.Responses;

namespace API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RepositoryResponse<TokenReturnDTO>> Login(LoginDTO loginDTO);
        Task<RepositoryResponse<UserDTO>> Register(RegisterDTO registerDTO);
        Task<RepositoryResponse<TokenDTO>> RefreshToken(TokenDTO tokenDTO);
        Task<RepositoryStatus> RevokeToken(string username);
    }
}
