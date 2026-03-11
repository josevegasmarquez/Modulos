using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Modulos.API.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombres { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Apellidos { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        [Required]
        public TipoGenero Genero { get; set; }
       [StringLength(200)]
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string FullName => $"{Nombres} {Apellidos}";
    }
    public enum TipoGenero
    {
        Masculino=0,
        Femenino=1,
        Otro=2,
        PrefieroNoDecirlo= 3
    }
}
