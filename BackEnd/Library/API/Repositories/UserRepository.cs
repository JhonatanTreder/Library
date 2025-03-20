using API.Context;
using API.DTO.Authentication;
using API.DTO.Login;
using API.DTO.User;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> UpdateUserRoleAsync(string id, string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) 
                return false;

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Contains(newRole)) 
                return true;

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
                return false;

            var addResult = await _userManager.AddToRoleAsync(user, newRole);

            return addResult.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<User> GetUserAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<bool> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return false;

            if (!string.IsNullOrEmpty(userUpdateDTO.Name))
                user.Name = userUpdateDTO.Name;

            if (!string.IsNullOrEmpty(userUpdateDTO.PhoneNumber))
                user.PhoneNumber = userUpdateDTO.PhoneNumber;

            if (!string.IsNullOrEmpty(userUpdateDTO.Email) && user.Email != userUpdateDTO.Email)
            {
                user.Email = userUpdateDTO.Email;
                user.NormalizedEmail = userUpdateDTO.Email.ToUpper();
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, userUpdateDTO.Password);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            var updatedResult = await _userManager.UpdateAsync(user);

            return updatedResult.Succeeded;
        }
    }
}
