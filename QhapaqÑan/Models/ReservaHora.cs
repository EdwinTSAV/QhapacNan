namespace QhapaqÑan.Models
{
    public class ReservaHora
    {
        public int Id { get; set; }
        public int Id_Reserva { get; set; }
        public int Id_Hora { get; set; }
        public Hora Hora { get; set; }
        public Reserva Reserva { get; set; }
    }
}
