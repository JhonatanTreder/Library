using API.DTO.Responses;
using API.DTO.User;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        public async Task<IActionResult> Get(string id)
        {
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
               Message = $"Usuário de id '{id}' encontrado com sucesso"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
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
    }
}
