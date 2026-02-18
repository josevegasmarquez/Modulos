namespace Modulos.API.Models
{
    public class HorarioSemanalViewModel
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdTurno { get; set; }
        public DayOfWeek Descanso { get; set; }
        public DateTime InicioSemana { get; set; }
        public DateTime FinSemana { get; set; }
    }
}
