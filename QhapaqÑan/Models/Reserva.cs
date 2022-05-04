using System;
using System.ComponentModel.DataAnnotations;

namespace QhapaqÑan.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public string DNI_User { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public DateTime Fecha_Pre { get; set; }
        public DateTime Fecha_Post { get; set; }
        public bool Estado { get; set; }
        public decimal Precio { get; set; }
    }
}
