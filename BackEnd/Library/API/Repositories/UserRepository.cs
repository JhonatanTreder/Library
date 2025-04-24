using API.DTO.Responses;
using API.DTO.User;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RepositoryStatus> UpdateUserRoleAsync(string id, string newRole)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RepositoryStatus.InvalidId;

            if (string.IsNullOrEmpty(newRole))
                return RepositoryStatus.InvalidRole;

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return RepositoryStatus.NotFound;

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Contains(newRole))
                return RepositoryStatus.AlreadyInRole;

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (removeResult.Succeeded is false)
                return RepositoryStatus.RoleRemovedFailed;

            var addResult = await _userManager.AddToRoleAsync(user, newRole);

            if (addResult.Succeeded is false)
                return RepositoryStatus.RoleUpdatedFailed;

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RepositoryStatus.InvalidId;

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return RepositoryStatus.UserNotFound;

            var deleted = await _userManager.DeleteAsync(user);

            if (deleted.Succeeded is false)
                return RepositoryStatus.Failed;

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryResponse<IEnumerable<UserFilterDTO>>> GetUsersAsync(UserFilterDTO userDTO)
        {
            if (userDTO is null)
            {
                return new RepositoryResponse<IEnumerable<UserFilterDTO>>(RepositoryStatus.NullObject);
            }

            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(userDTO.Name))
                query = query.Where(n => n.Name == userDTO.Name);

            if (!string.IsNullOrEmpty(userDTO.Email))
                query = query.Where(n => n.Email == userDTO.Email);

            if (userDTO.UserType != null)
                query = query.Where(t => t.UserType == userDTO.UserType);

            var queryUsers = await query.ToListAsync();

            var users = queryUsers.Select(u => new UserFilterDTO
            {
                Name = u.Name,
                Email = u.Email,
                UserType = u.UserType
            });

            if (users.Any() is false)
            {
                return new RepositoryResponse<IEnumerable<UserFilterDTO>>(RepositoryStatus.NotFound);
            }

            else
            {
                return new RepositoryResponse<IEnumerable<UserFilterDTO>>(RepositoryStatus.Success, users);
            }
        }

        public async Task<RepositoryResponse<UserDTO>> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidId);
            }

            var dbUser = await _userManager.FindByIdAsync(userId);

            if (dbUser is null)
            {
                return new RepositoryResponse<UserDTO>(RepositoryStatus.UserNotFound);
            }

            var user = new UserDTO
            {
                Name = dbUser.Name,
                Email = dbUser.Email,
                PhoneNumber = dbUser.PhoneNumber,
                UserType = dbUser.UserType
            };

            return new RepositoryResponse<UserDTO>(RepositoryStatus.Success, user);
        }

        public async Task<RepositoryStatus> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO)
        {
            if (userUpdateDTO is null)
                return RepositoryStatus.NullObject;

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return RepositoryStatus.NotFound;

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
                    return RepositoryStatus.FailedToResetPassword;
                }
            }

            var updatedResult = await _userManager.UpdateAsync(user);

            if (updatedResult.Succeeded)
                return RepositoryStatus.Failed;

            return RepositoryStatus.Success;
        }
    }
}
