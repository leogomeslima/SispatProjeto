using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sispat.Application.DTOs;
using Sispat.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sispat.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Email já cadastrado." });
            }

            var newUser = new ApplicationUser
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.Email // UserName é obrigatório
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = errors });
            }

            // Opcional: Logar o usuário automaticamente após o registro
            var (token, expiration) = GenerateJwtToken(newUser);

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Usuário registrado com sucesso!",
                Token = token,
                TokenExpiration = expiration,
                FullName = newUser.FullName,
                Email = newUser.Email
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = "Credenciais inválidas." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = "Credenciais inválidas." });
            }

            var (token, expiration) = GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login bem-sucedido.",
                Token = token,
                TokenExpiration = expiration,
                FullName = user.FullName,
                Email = user.Email
            });
        }

        private (string token, DateTime expiration) GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("uid", user.Id), // Claim customizada com o ID do usuário
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(8); // Token válido por 8 horas

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }
    }
}
