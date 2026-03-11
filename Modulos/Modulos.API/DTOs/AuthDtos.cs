using Modulos.API.Models;
using System.ComponentModel.DataAnnotations;

namespace Modulos.API.DTOs
{
    public class RegisterRequest
    {
        [Required,StringLength(50)]
        public string Nombres { get; set; } = string.Empty;
        [Required,StringLength(50)]
        public string Apellidos { get; set; } = string.Empty;
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required,MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required,DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        [Required]
        public TipoGenero Genero { get; set; }
        public string? Direccion { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public UserDto? User { get; set; }
    }
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
    public class UpdateProfileRequest
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public TipoGenero Genero { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
