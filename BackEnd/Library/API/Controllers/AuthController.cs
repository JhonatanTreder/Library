using API.DTO.Login;
using API.DTO.Responses;
using API.DTO.Token;
using API.DTO.User;
using API.Enum;
using API.Enum.Responses;
using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var response = await _authService.Login(loginDTO);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Usuário logado com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A requisição de login não pode ser nula"
                }),

                RepositoryStatus.FailedToUpdateUser => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao atualizar o usuário com as novas informações do Refresh Token"
                }),

                RepositoryStatus.Unauthorized => Unauthorized(new ApiResponse
                {
                    Status = "Unauthorized",
                    Data = null,
                    Message = "Credenciais inválidas"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar logar o usuário"
                })
            };
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var response = await _authService.Register(registerDTO);

            return response.Status switch 
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Usuário registardo com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A requisição de registro não pode ser nula"
                }),

                RepositoryStatus.EmailAlreadyExists => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "O Email especificado já está sendo utilizado"
                }),

                RepositoryStatus.InvalidPassword => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A senha deve ter no mínimo 6 caracteres"
                }),

                RepositoryStatus.FailedToCreateUser => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar criar um usuário"
                }),

                RepositoryStatus.FailedToCreateRole => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar criar a role para o usuário"
                }),

                RepositoryStatus.InvalidRoleAssignment => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar atribuir uma role para o usuário"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao registrar o usuário"
                })
            };
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDTO tokenDTO)
        {
            var response = await _authService.RefreshToken(tokenDTO);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Refresh Token criado com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O token não pode ser nulo"
                }),

                RepositoryStatus.InvalidRefreshToken => Unauthorized(new ApiResponse
                {
                    Status = "Unauthorized",
                    Data = null,
                    Message = "Token/Refresh Token inválidos"
                }),

                RepositoryStatus.InvalidRefreshTokenExpiryTime => Unauthorized(new ApiResponse
                {
                    Status = "Unauthorized",
                    Data = null,
                    Message = "O tempo de expiração do Refresh Token está inválido"
                }),

                RepositoryStatus.FailedToUpdateUser => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Falha ao atualizar o usuário com o novo Refresh Token"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao gerar o Refresh Token"
                })
            };
        }

        [HttpPut]
        [Authorize()]
        [Route("revoke/{username}")]
        public async Task<IActionResult> RevokeToken(string username)
        {
            var response = await _authService.RevokeToken(username);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.UserNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O usuário '{username}' não foi encontrado"
                }),

                RepositoryStatus.FailedToRevokeToken => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = $"Falha ao revogar o token para o usuário '{username}'"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = $"Erro inesperado ao revogar o token para o usuário '{username}'"
                })
            };
        }
    }
}
