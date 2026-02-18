using System.ComponentModel.DataAnnotations;

namespace Modulos.API.Models
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El usuario es obligatorio")]
        public string Username { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string PasswordHash { get; set; }
    }
}
