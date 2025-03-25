using API.Context;
using API.DTO.Authentication;
using API.DTO.Login;
using API.DTO.User;
using API.Enum;
using API.Enum.Responses;
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

        public async Task<UserResponse> UpdateUserRoleAsync(string id, string newRole)
        {
            if (string.IsNullOrWhiteSpace(id))
                return UserResponse.InvalidId;

            if (string.IsNullOrEmpty(newRole))
                return UserResponse.InvalidRole;

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return UserResponse.NotFound;

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Contains(newRole))
                return UserResponse.AlreadyInRole;

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (removeResult.Succeeded is false)
                return UserResponse.RoleRemovedFailed;

            var addResult = await _userManager.AddToRoleAsync(user, newRole);

            if (addResult.Succeeded is false)
                return UserResponse.RoleUpdatedFailed;

            return UserResponse.Success;
        }

        public async Task<UserResponse> DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return UserResponse.InvalidId;

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return UserResponse.NotFound;

            var deleted = await _userManager.DeleteAsync(user);

            if (deleted.Succeeded is false)
                return UserResponse.Failed;

            return UserResponse.Success;
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

        public async Task<UserResponse> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO)
        {
            if (userUpdateDTO is null)
                return UserResponse.NullObject;

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return UserResponse.NotFound;

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
                var resetPassword = await _userManager.ResetPasswordAsync(user, token, userUpdateDTO.Password);

                if (resetPassword.Succeeded is false)
                {
                    return UserResponse.FailedToResetPassword;
                }
            }

            var updatedResult = await _userManager.UpdateAsync(user);

            if (updatedResult.Succeeded)
                return UserResponse.Failed;

            return UserResponse.Success;
        }
    }
}
