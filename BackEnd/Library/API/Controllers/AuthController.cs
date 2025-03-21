using API.DTO.Login;
using API.DTO.Responses;
using API.DTO.Token;
using API.DTO.User;
using API.Enum;
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
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email!);

            if (user is not null && await _userManager.CheckPasswordAsync(user, loginDTO.Password!))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.UserType.ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _tokenService.GenerateAccessToken(authClaims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

                await _userManager.UpdateAsync(user);

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }

            return Unauthorized(new ApiResponse
            {
                Status = "Unauthorized",
                Message = "Usuário não autorizado"
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO userRegisterDTO)
        {
            var userExists = await _userManager.FindByEmailAsync(userRegisterDTO.Email);

            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                new ApiResponse
                {
                    Status = "Conflict",
                    Message = "Já existe um usuário com este email"
                });
            }

            if (userRegisterDTO.Password.Length < 6)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "A quantidade de caracteres para a senha é de no mínimo 6"
                });
            }

            ApplicationUser user = new()
            {
                Email = userRegisterDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userRegisterDTO.Name
            };

            var result = await _userManager.CreateAsync(user, userRegisterDTO.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Falha ao criar um usuário"
                });
            }

            var roleName = UserType.User.ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (!roleResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                    {
                        Status = "Internal Server Error",
                        Message = "Falha ao criar a role do usuário"
                    });
                }
            }

            var roleAssignmentResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!roleAssignmentResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Falha ao associar um usuário à role"
                });
            }

            return Ok(
            new ApiResponse
            {
                Status = "Ok",
                Data = new UserDTO
                {
                    Name = userRegisterDTO.Name,
                    Email = userRegisterDTO.Email,
                    PhoneNumber = userRegisterDTO.PhoneNumber,
                    UserType = UserType.User
                },
                Message = "Usuário criado com sucesso!"
            });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDTO tokenDTO)
        {
            if (tokenDTO is null)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "Requisição inválida do cliente"
                });
            }

            string? accessToken = tokenDTO.AccessToken ?? throw new ArgumentNullException(nameof(tokenDTO));
            string? refreshToken = tokenDTO.RefreshToken ?? throw new ArgumentNullException(nameof(tokenDTO));

            var principal = _tokenService.GetClaimsFromExpiredToken(accessToken!);

            if (principal is null)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "Access/refresh token inválido"
                });
            }

            string username = principal.Identity!.Name!;

            var user = await _userManager.FindByNameAsync(username!);

            if (user is null ||
                string.IsNullOrEmpty(user.RefreshToken) ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "Access/Refresh token inválido"
                });
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList());
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Status = "Internal Server Error",
                    Message = "Falha ao atualizar um usuário"
                });
            }

            return Ok(new
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefeshToken = newRefreshToken
            });
        }

        [HttpPut]
        [Authorize()]
        [Route("revoke/{username}")]
        public async Task<IActionResult> RevokeToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                return NotFound(new ApiResponse
                {
                    Status = "Not Found",
                    Message = $"Usuário '{username}' não encontrado"
                });
            }

            user.RefreshToken = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Bad Request",
                    Message = "Falha ao revogar o token"
                });
            }

            return NoContent();
        }
    }
}
