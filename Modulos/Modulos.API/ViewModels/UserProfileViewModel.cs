using Modulos.API.Models;
using System.ComponentModel.DataAnnotations;

namespace Modulos.API.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="El nombre es obligatorio")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellidos { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public TipoGenero Genero { get; set; }
        [Phone]
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
