using System.ComponentModel.DataAnnotations;
using Modulos.API.Models;

namespace Modulos.API.ViewModels
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength=2)]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, MinimumLength =2)]
        public string Apellidos { get; set; }
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Email no válido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        [Required(ErrorMessage = "El género es obligatorio.")]
        public TipoGenero Genero { get; set; }
        [Phone(ErrorMessage = "Número de teléfono no válido.")]
        public string Telefono { get; set; }
        [StringLength(200)]
        public string Direccion { get; set; }
    }

}
