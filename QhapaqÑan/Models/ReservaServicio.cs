namespace QhapaqÑan.Models
{
    public class ReservaServicio
    {
        public int Id { get; set; }
        public int Id_Reserva { get; set; }
        public int Id_Servicio { get; set; }
        public Servicios Servicios { get; set; }
        public Reserva Reserva { get; set; }
    }
}
