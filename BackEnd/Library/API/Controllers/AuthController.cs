using API.DTOs.Authentication;
using API.DTOs.Responses;
using API.DTOs.Token;
using API.Enum.Responses;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

                RepositoryStatus.Unauthorized => Unauthorized(new ApiResponse
                {
                    Status = "Unauthorized",
                    Data = null,
                    Message = "Credenciais inválidas"
                }),

                RepositoryStatus.FailedToUpdateUser => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao atualizar o usuário com as novas informações do Refresh Token"
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var response = await _authService.Register(registerDTO);

            return response.Status switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = response.Data,
                    Message = "Usuário registrado com sucesso"
                }),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A requisição de registro não pode ser nula"
                }),

                RepositoryStatus.InvalidDomain => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = $"O domínio do email '{registerDTO.Email}' do usuário é inválido"
                }),

                RepositoryStatus.InvalidPassword => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "A senha deve ter no mínimo 6 caracteres"
                }),

                RepositoryStatus.InvalidMatriculatesFormat => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O número de matrícula está em um formato inválido"
                }),

                RepositoryStatus.EmailAlreadyExists => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "O Email especificado já está sendo utilizado"
                }),

                RepositoryStatus.MatriculatesAlreadyExists => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "A matrícula especificada já está sendo utilizada"
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
        [Route("email-confirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmEmail([FromBody] string email)
        {
            var response = await _authService.SendEmailConfirmationAsync(email);
            Console.WriteLine(response);

            return response switch 
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = "Email enviado com sucesso"
                }),

                RepositoryStatus.UserNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = $"O usuário de email '{email}' não foi encontrado"
                }),

                RepositoryStatus.FailedToUpdateUser => Conflict(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = "Erro inesperado ao tentar atualizar o usuário"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar enviar a confirmação de email para o usuário"
                })
            };
        }

        [HttpPost]
        [Route("phone-confirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmPhone([FromBody] string email)
        {
            var response = await _authService.SendPhoneConfirmationAsync(email);

            return response switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = "Código de confirmação de telefone enviado com sucesso"
                }),

                RepositoryStatus.UserNotFound => NotFound(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = $"O usuário com o email '{email}' não foi encontrado"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = "O número de telefone do usuário não foi encontrado"
                }),

                RepositoryStatus.InvalidPhoneFormat => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "O número de telefone está em um formato inválido"
                }),

                RepositoryStatus.FailedToUpdateUser => Conflict(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = "Erro inesperado ao tentar atualizar o usuário"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao tentar processar o código de confirmação do telefone"
                })
            };
        } 

        [HttpPost]
        [Route("verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCodeDTO verifyEmailDTO)
        {
            var response = await _authService.VerifyEmailCodeAsync(verifyEmailDTO);

            return response switch 
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message  = "Código verificado com sucesso"
                }),

                RepositoryStatus.InvalidConfirmationCode => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O código de confirmação do usuário está inválido"
                }),

                RepositoryStatus.FailedToUpdateUser => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "Ocorreu um erro inesperado ao atualizar o usuário com as novas informações do email"
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
                    Message = "Erro inesperado ao verificar o código de confirmação do usuário"
                })
            };
        }

        [HttpPost]
        [Route("verify-phone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneCodeDTO verifyPhoneDTO)
        {
            var response = await _authService.VerifyPhoneCodeAsync(verifyPhoneDTO);

            return response switch
            {
                RepositoryStatus.Success => Ok(new ApiResponse
                {
                    Status = "Ok",
                    Data = null,
                    Message = "Código de telefone verificado com sucesso"
                }),

                RepositoryStatus.UserNotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "O usuário não foi encontrado"
                }),

                RepositoryStatus.NotFound => NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Data = null,
                    Message = "O número de telefone não foi encontrado"
                }),

                RepositoryStatus.InvalidConfirmationCode => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O código e confirmação está inválido"
                }),

                RepositoryStatus.InvalidPhoneFormat => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O número de telefone do usuário está em um formato inválido"
                }),

                RepositoryStatus.ConfirmationCodeExpired => StatusCode(StatusCodes.Status410Gone, new ApiResponse
                {
                    Status = "Gone",
                    Data = null,
                    Message = "O código de confirmação foi expirado"
                }),

                RepositoryStatus.FailedToUpdateUser => Conflict(new ApiResponse
                {
                    Status = "Conflict",
                    Data = null,
                    Message = "Erro inesperado ao tentar atualizar o usuário"
                }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Data = null,
                    Message = "Erro inesperado ao verificar o número de telefone do usuário"
                }),
            };
        }

        [HttpPost]
        [Route("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RevokeToken(string username)
        {
            var response = await _authService.RevokeToken(username);

            return response switch
            {
                RepositoryStatus.Success => NoContent(),

                RepositoryStatus.NullObject => BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Data = null,
                    Message = "O nome do usuário não pode ser nulo ou conter espaços vazios"
                }),

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
