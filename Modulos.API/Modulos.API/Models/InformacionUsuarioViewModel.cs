namespace Modulos.API.Models
{
    public class InformacionUsuarioViewModel
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion {  get; set; }
        public DateTime FechaNacimiento {  get; set; }
        public bool Sexo {  get; set; }
    }
}
