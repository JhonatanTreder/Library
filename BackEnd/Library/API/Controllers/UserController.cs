using API.DTOs.Responses;
using API.DTOs.User;
using API.Enum.Responses;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] UserFilterDTO userFilterDTO)
        {
            var response = await _userRepository.GetUsersAsync(userFilterDTO);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Usuários encontrados com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "Filtro de pesquisa inválido"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "Nenhum usuário foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao buscar usuários"
                })
            };
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string id)
        {
            var response = await _userRepository.GetUserByIdAsync(id);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = $"Usuário de id '{id}' encontrado com sucesso"
                }),

                RepositoryStatus.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O id do usuário não pode ser nulo ou vazio"
                }),

                RepositoryStatus.UserNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O usuário de id '{id}' não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao buscar um usuário"
                })
            };
        }

        [HttpGet("email/{email}")]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var response = await _userRepository.GetUserByEmailAsync(email);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Usuário encontrado com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O email do usuário não pode ser nulo"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "O usuário não foi encontrado"
                }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao buscar o usuário pelo email"
                })
            };
        }

        [HttpGet("{userId}/user-dashboard")]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserDashboard(string userId)
        {
            var response = await _userRepository.GetGeneralUserInfoAsync(userId);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Informações gerais do usuário encontradas com sucesso"
                }),

                RepositoryStatus.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = "O id do usuário não pode ser nulo ou onter espaços em branco"
                }),

                RepositoryStatus.UserNotFound => NotFound(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = "O usuário não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar buscar pelas informações do dashboard do usuário"
                })
,
            };
        }

        [HttpGet]
        [Authorize(Roles = "user,librarin,admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPendingValidations(string userId)
        {
            var response = await _userRepository.GetPendingValidations(userId);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Validações pendentes encontradas com sucesso"
                }),

                RepositoryStatus.UserNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "O usuário não foi encontrado"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao buscar pelas validações pendentes do usuário"
                })
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _userRepository.DeleteUserAsync(id);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O id do usuário não pode ser nulo ou conter espaços em brancos"
                }),

                RepositoryStatus.UserNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O usuário de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.Failed => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = $"Erro inesperado ao tentar deletar o usuário de id '{id}'"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar deletar o usuário"
                })
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "user,librarian,admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(string id, UserUpdateDTO userUpdateDTO)
        {
            var response = await _userRepository.UpdateUserAsync(id, userUpdateDTO);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O usuário de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O usuário não pode ser nulo"
                }),

                RepositoryStatus.InvalidPhoneFormat => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O número de telefone do usuário deve estar em um formato válido (formato E.164)"
                }),

                RepositoryStatus.FailedToResetPassword => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "Erro inesperado ao tentar atualizar a senha do usuário"
                }),

                RepositoryStatus.Failed => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "Erro inesperado ao tentar atualizar o usuário"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar atualizar um usuário"
                })
            };
        }

        [HttpPut("{id}/role/{newRole}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(string id, string newRole)
        {
            var response = await _userRepository.UpdateUserRoleAsync(id, newRole);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.AlreadyInRole => NoContent(),

                RepositoryStatus.InvalidId => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O id do usuário não pode ser nulo ou conter espaços em branco"
                }),

                RepositoryStatus.InvalidRole => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A role do usuário não pode ser nula ou vazia"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O usuário de id '{id}' não foi encontrado"
                }),

                RepositoryStatus.RoleRemovedFailed => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "Erro inesperado ao tentar remover a role antiga do usuário"
                }),

                RepositoryStatus.RoleUpdatedFailed => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "Erro inesperado ao tentar atribuir o usuário a uma nova role"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar atualizar a role do usuário"
                })
            };
        }
    }
}
