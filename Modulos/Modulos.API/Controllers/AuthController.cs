using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modulos.API.DTOs;
using Modulos.API.Models;
using Modulos.API.Services;

namespace Modulos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized(new AuthResponse { Success = false, Message = "Invalid credentials" });
            }

            var token = await _jwtService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Success = true,
                Token = token,
                User = await MapToDto(user)
            });
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null) return BadRequest(new AuthResponse { Success = false, Message = "Email already in use" });

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                DNI = request.DNI,
                FechaNacimiento = request.FechaNacimiento,
                Genero = request.Genero,
                Direccion = request.Direccion,
                PhoneNumber = request.PhoneNumber,
                MustChangePassword = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponse { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
            }

            var role = string.IsNullOrEmpty(request.Role) ? "User" : request.Role;
            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return Ok(new AuthResponse { Success = true, Message = "User registered successfully", User = await MapToDto(user) });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<AuthResponse>> ChangePassword(ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new AuthResponse { Success = false, Message = "Passwords do not match" });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponse { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) });
            }

            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponse { Success = true, Message = "Password changed successfully" });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<AuthResponse>> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.Nombres = request.FirstName;
            user.Apellidos = request.LastName;
            user.FechaNacimiento = request.DateOfBirth;
            user.Genero = request.Genero;
            user.PhoneNumber = request.PhoneNumber;
            user.Direccion = request.Address;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponse { Success = false, Message = "Update failed" });
            }

            return Ok(new AuthResponse { Success = true, User = await MapToDto(user) });
        }

        private async Task<UserDto> MapToDto(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.Nombres,
                LastName = user.Apellidos,
                FullName = user.FullName,
                DNI = user.DNI,
                DateOfBirth = user.FechaNacimiento,
                Genero = user.Genero.ToString(),
                PhoneNumber = user.PhoneNumber,
                Address = user.Direccion,
                RegistrationDate = user.FechaRegistro,
                MustChangePassword = user.MustChangePassword,
                Roles = roles.ToList()
            };
        }
    }
}
