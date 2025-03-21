using API.DTO.Responses;
using API.DTO.User;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get([FromQuery] UserFilterDTO userFilterDTO)
        {
            if (userFilterDTO == null)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "Filtro de pesquisa inválido"
                });
            }

            var users = await _userRepository.GetUsersAsync(userFilterDTO);

            if (users is null || !users.Any())
            {
                return NotFound( new ApiResponse
                {
                    Status = "Not Found",
                    Message = "Usuários não encontrados com os critérios fornecidosx"
                });
            }

            return Ok(new ApiResponse 
            {
                Status = "Ok",
                Data = users,
                Message = "Usuários encontrados com sucesso"
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ApiResponse 
                {
                    Status = "Bad Request",
                    Message = "O id do usuário não pode ser nulo ou vazio"
                });
            }

            var user = await _userRepository.GetUserByIdAsync(id);

            if (user is null)
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"Usuário de id '{id}' não encontrado"
                });
            }

            return Ok(new ApiResponse
            {
                Status = "Ok",
                Data = new UserDTO
                {
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType,
                },
                Message = $"Usuário de id '{id}' encontrado com sucesso"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "O id do usuário não pode ser nulo ou vazio"
                });
            }

            var deleted = await _userRepository.DeleteUserAsync(id);

            if (deleted is false)
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"Usuário de id '{id}' não encontrado"
                });
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Put(string id, UserUpdateDTO userUpdateDTO)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "O id do usuário não pode ser nulo ou vazio"
                });
            }

            var updated = await _userRepository.UpdateUserAsync(id, userUpdateDTO);

            if (updated is false)
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"Usuário de id '{id}' não encontrado"
                });
            }

            return NoContent();
        }

        [HttpPut("{id}/role/{newRole}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Put(string id, string newRole)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(newRole)) 
            {
                return BadRequest(new ApiResponse 
                {
                    Status = "Bad Request",
                    Message = "O id do usuário ou a nova role não podem ser nulos ou vazios"
                });
            }

            var updated = await _userRepository.UpdateUserRoleAsync(id, newRole);

            if (!updated)
            {
                return NotFound(new ApiResponse 
                {
                    Status = "Internal Server Error",
                    Message = $"Usuário com id '{id}' não encontrado ou falha ao atualizar uma role"
                });
            }

            return NoContent();
        }
    }
}
