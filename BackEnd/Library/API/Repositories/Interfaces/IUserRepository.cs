﻿using API.DTOs.Responses;
using API.DTOs.User;
using API.Enum.Responses;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<RepositoryStatus> UpdateUserRoleAsync(string id, string newRole);
        Task<RepositoryResponse<IEnumerable<UserFilterDTO>>> GetUsersAsync(UserFilterDTO userDTO);
        Task<RepositoryResponse<UserDTO>> GetUserByIdAsync(string id);
        Task<RepositoryStatus> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO);
        Task<RepositoryStatus> DeleteUserAsync(string id);
    }
}
