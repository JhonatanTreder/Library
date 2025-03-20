using API.Context;
using API.DTO.Authentication;
using API.DTO.Login;
using API.DTO.User;
using API.Enum;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        public async Task<IEnumerable<UserFilterDTO?>> GetUsersAsync(UserFilterDTO userDTO)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(userDTO.Name))
                query = query.Where(n => n.Name == userDTO.Name);

            if (!string.IsNullOrEmpty(userDTO.Email))
                query = query.Where(n => n.Email == userDTO.Email);

            if (userDTO.UserType != null)
                query = query.Where(t => t.UserType == userDTO.UserType);

            var users = await query.ToListAsync();

            return users.Select(u => new UserFilterDTO 
            {
                Name = u.Name,
                Email = u.Email,
                UserType = u.UserType
            });
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
