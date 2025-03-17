using API.DTO.Login;
using API.DTO.Responses;
using API.DTO.Token;
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
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;

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

        [HttpPut]
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
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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

            return Unauthorized();
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
                    Status = "Error",
                    Message = "User already exists"
                });
            }

            ApplicationUser user = new()
            {
                Email = userRegisterDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userRegisterDTO.Name
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse
                {
                    Status = "Error",
                    Message = "User creation failed"
                });
            }

            return Ok(
            new ApiResponse
            {
                Status = "Success",
                Message = "User created successfully!"
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
                    Status = "Error",
                    Message = "Invalid client request"
                });
            }

            string? accessToken = tokenDTO.AccessToken ?? throw new ArgumentNullException(nameof(tokenDTO));
            string? refreshToken = tokenDTO.RefreshToken ?? throw new ArgumentNullException(nameof(tokenDTO));

            var principal = _tokenService.GetClaimsFromExpiredToken(accessToken!);

            if (principal is null)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Error",
                    Message = "Invalid access/refresh token"
                });
            }

            string username = principal.Identity!.Name!;

            var user = await _userManager.FindByNameAsync(username!);

            if (user is null || user.RefreshToken != refreshToken 
                             || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest(new ApiResponse 
                {
                    Status = "Error",
                    Message = "Invalid access/refresh token"
                });
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList());
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken)
            });
        }

        [HttpPut]
        [Authorize]
        [Route("revoke/{username}")]
        public async Task<IActionResult> RevokeToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null) 
            {
                return NotFound(new ApiResponse 
                {
                    Status = "Not found",
                    Message = $"User '{username}' not found"
                }); 
            }

            user.RefreshToken = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse 
                {
                    Status = "Error",
                    Message = "Failed to revoke token"
                });
            }

            return Ok(new ApiResponse 
            {
                Status = "Success",
                Message = $"Token revoked for user '{username}'"
            });
        }
    }
}
