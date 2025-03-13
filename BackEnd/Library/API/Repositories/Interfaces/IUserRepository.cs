using API.DTO.Authentication;
using API.DTO.Login;
using API.DTO.User;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(RegisterDTO user);
        Task<User> AddUserAsync(AdminRegisterDTO user);
        Task<User> GetUserAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO userUpdateDTO);
        Task<bool> DeleteUserAsync(int id);
    }
}
