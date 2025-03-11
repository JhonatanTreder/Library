using API.Context;
using API.DTO.User;
using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<User> AddUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);

            if (user is null )
            {
                return false;
            }

            _context.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public Task<User> GetUserAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.AppUsers.FindAsync(id);
        }

        public Task<bool> UpdateUserAsync(int id, UserUpdateDTO userUpdateDTO)
        {
            throw new NotImplementedException();
        }
    }
}
