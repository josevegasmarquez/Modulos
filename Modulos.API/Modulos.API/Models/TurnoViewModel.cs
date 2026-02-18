namespace Modulos.API.Models
{
    public class TurnoViewModel
    {
        public int Id { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public string Descripcion { get; set; }
    }
}
