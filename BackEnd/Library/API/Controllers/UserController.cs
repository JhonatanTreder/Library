using API.DTO.Responses;
using API.DTO.User;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            var response = await _userRepository.DeleteUserAsync(id);

            return response switch 
            {
                UserResponse.Success => NoContent(),

                UserResponse.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "O id do usuário não pode ser nulo ou conter espaços em brancos"
                }),

                UserResponse.NotFound => NotFound(new ApiResponse 
                {
                    Status = "Not Found",
                    Message = "O usuário de id '{id}' não foi encontrado"
                }),

                UserResponse.Failed => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Message = $"Erro ao tentar deletar o usuário de id '{id}'"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao tentar deletar o usuário"
                })
            };
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

            var response = await _userRepository.UpdateUserAsync(id, userUpdateDTO);

            return response switch 
            {
                UserResponse.Success => NoContent(),

                UserResponse.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"O usuário de id '{id}' não foi encontrado"
                }),

                UserResponse.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "O usuário não pode ser nulo"
                }),

                UserResponse.FailedToResetPassword => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Message = "Erro inesperado ao tentar atualizar a senha do usuário"
                }),

                UserResponse.Failed => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Message = "Erro inesperado ao tentar atualizar o usuário"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Erro inesperado ao tentar atualizar um usuário"
                })
            };
        }

        [HttpPut("{id}/role/{newRole}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Put(string id, string newRole)
        {
            var response = await _userRepository.UpdateUserRoleAsync(id, newRole);
                
                return response switch 
                {
                    UserResponse.Success => NoContent(),

                    UserResponse.AlreadyInRole => NoContent(),

                    UserResponse.InvalidId => BadRequest(new ApiResponse
                    {
                        Status = "Bad Request",
                        Message = "O id do usuário não pode ser nulo ou conter espaços em branco"
                    }),

                    UserResponse.InvalidRole => BadRequest(new ApiResponse
                    {
                        Status = "Bad Request",
                        Message = "A role do usuário não pode ser nula ou vazia"
                    }),

                    UserResponse.NotFound => NotFound(new ApiResponse
                    {
                        Status = "Not Found",
                        Message = "O usuário de id '{id}' não foi encontrado"
                    }),

                    UserResponse.RoleRemovedFailed => Conflict(new ApiResponse
                    {
                        Status = "Erro inesperado ao tentar remover a role antiga do usuário"
                    }),

                    UserResponse.RoleUpdatedFailed => Conflict(new ApiResponse
                    {
                        Status = "Conflict",
                        Message = "Erro inesperado ao tentar atribuir o usuário a uma nova role"
                    }),

                    _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                    {
                        Status = "Internal Server Error",
                        Message = "Erro inesperado ao tentar atualizar a role do usuário"
                    })
                };
        }
    }
}
