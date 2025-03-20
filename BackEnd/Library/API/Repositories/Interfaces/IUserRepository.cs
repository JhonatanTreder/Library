using API.DTO.Authentication;
using API.DTO.Login;
using API.DTO.User;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UpdateUserRoleAsync(string id, string newRole);
        Task<User> GetUserAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO);
        Task<bool> DeleteUserAsync(string id);
    }
}
