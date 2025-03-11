using API.DTO.User;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User> GetUserAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO userUpdateDTO);
        Task<bool> DeleteUserAsync(int id);
    }
}
