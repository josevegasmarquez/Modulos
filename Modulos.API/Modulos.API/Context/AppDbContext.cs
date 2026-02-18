using Microsoft.EntityFrameworkCore;
using Modulos.API.Models;

namespace Modulos.API.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        //Crear una tabla llamada Usuarios
        public DbSet<UsuarioViewModel> Usuarios { get; set; }
        public DbSet<InformacionUsuarioViewModel> InformacionUsuarios { get; set; }
        public DbSet<TurnoViewModel> Turnos { get; set; }
        public DbSet<HorarioSemanalViewModel> Horarios { get; set; }
    }
}
