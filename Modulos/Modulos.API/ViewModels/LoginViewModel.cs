using System.ComponentModel.DataAnnotations;

namespace Modulos.API.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage="El email es obligatorio")]
        [EmailAddress(ErrorMessage ="Email no válido")]
        public string Email { get; set; }
        [Required(ErrorMessage ="La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
