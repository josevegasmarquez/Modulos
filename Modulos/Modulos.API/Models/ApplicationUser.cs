using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace Modulos.API.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required(ErrorMessage= "El nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombres {  get; set; }
        [Required(ErrorMessage= "El apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellidos { get; set; }
        public string? Telefono {  get; set; }
        [StringLength(200)]
        public string? Direccion {  get; set; }
        [Required]
        public TipoGenero Genero { get; set; }
        [Required(ErrorMessage= "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }= DateTime.Now;
        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
    public enum TipoGenero
    {
        Masculino,
        Femenino,
        Otro,
        PrefieroNoDecir
    }
}
