using API.DTO.Authentication;
using API.DTO.Login;
using API.DTO.User;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserResponse> UpdateUserRoleAsync(string id, string newRole);
        Task<IEnumerable<UserFilterDTO?>> GetUsersAsync(UserFilterDTO userDTO);
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<UserResponse> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO);
        Task<UserResponse> DeleteUserAsync(string id);
    }
}
