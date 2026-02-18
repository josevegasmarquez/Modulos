using Microsoft.AspNetCore.Mvc;
using Modulos.API.Context;
using Modulos.API.Models;

namespace Modulos.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) 
        {
            _db = db;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(string user, string password)
        {
            var passwordHash= BCrypt.Net.BCrypt.HashPassword(password);
            var nuevoUsuario= new UsuarioViewModel { Username = user, PasswordHash=passwordHash };
            _db.Usuarios.Add(nuevoUsuario);
            await _db.SaveChangesAsync();//guarda en la db
            return Ok("Usuario registrado en la base de datos");
        }
        [HttpPost("login")]
        public IActionResult Login(string user, string password) 
        {
            var usuario=_db.Usuarios.FirstOrDefault(us=>us.Username==user);
            if (usuario == null|| !BCrypt.Net.BCrypt.Verify(password,usuario.PasswordHash))
            {
                return Unauthorized("Credenciales inválidas.");
            }
            return Ok("¡Bienvenido!");
        }
    }
}
